using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAOServer.Game;
using RAOServer.Game.PlayerStuff;
using RAOServer.Network;
using RAOServer.Utils;
using WebSocketSharp.Server;
using Timer = System.Timers.Timer;

namespace RAOServer {
    internal class RAOServer {
        private static RAOServer _instance;

        private readonly List<Player> _allPlayers;
        private readonly List<RAORoom> _serverRooms;
        private Thread _serverConnections;
        private Thread _serverConsole;
        private WebSocketServer _webSocketServer;

        private RAOServer() {
            // Init all lists
            _serverRooms = new List<RAORoom>();
            _allPlayers = new List<Player>();

            var timer = new Timer(5000);
            timer.Elapsed += OnTimedEvent;
            timer.Start();
            GC.KeepAlive(timer);
        }

        public static RAOServer Instance {
            get { return _instance ?? (_instance = new RAOServer()); }
        }

        public void Start() {
            // Init all services
            _webSocketServer = new WebSocketServer(Settings.Port);
            _webSocketServer.AddWebSocketService<RAOConnection>(Settings.GameRoute);

            _serverConnections = new Thread(ServerConnectionsHandler);
            _serverConnections.Start();

            _serverConsole = new Thread(ServerConsoleHandler);
            _serverConsole.Start();

            CreateNewRoom(4, 250);

            _serverConsole.Join();
            _serverConnections.Join();
        }

        public int CreateNewRoom(int maxPlayers, int turnTime) {
            var newRoom = new RAORoom(maxPlayers, turnTime);
            _serverRooms.Add(newRoom);
            Log.Debug("Creating new room");
            return newRoom.Id;
        }

        /// <summary>
        ///     Переодически вызывается серверов
        /// </summary>
        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            CheckPlayersOnline();
        }


        public WebSocketServer GetSocketServer() {
            return _webSocketServer;
        }

        public List<RAORoom> GetRooms() {
            return _serverRooms;
        }

        public RAORoom GetRoom(int index) {
            var room = _serverRooms.Find(rm=>rm.Id == index);
            if (room == null){
                throw new InvalidDataValues();
            }
            return room;
        }

        public List<Player> GetPlayers() {
            return _allPlayers;
        }

        private void ServerConsoleHandler(object obj) {
            var serverConsole = new ServerConsole(this);
            serverConsole.Start();
        }

        private void ServerConnectionsHandler(object obj) {
            Log.Network("Starting Listen connections on " + Settings.Ip + ":" + Settings.Port);

            // For the information about this: https://github.com/sta/websocket-sharp           
            _webSocketServer.Start();
        }

        public void CheckPlayersOnline() {
            var ids = _webSocketServer.WebSocketServices[Settings.GameRoute].Sessions.IDs;
            foreach (var player in _allPlayers.ToList()){
                if (!ids.Contains(player.Id)){
                    RemovePlayer(player.Id);
                }
            }
        }

        public Player RegisterPlayer(string id, string login, RAOConnection connection) {
            var pl = new Player(id, login, connection);
            _allPlayers.Add(pl);

            Log.Game(string.Format("Player {0} joined to the game", pl.Name));

            return pl;
        }

        public void RemovePlayer(string id) {
            var pl = _allPlayers.Find(player=>player.Id == id);
            if (pl != null){
                _allPlayers.Remove(pl);

                if (pl.CurrentRoom != null)
                    pl.CurrentRoom.DisconnectPlayer(pl);
                Log.Game(string.Format("Player {0} has disconnected", pl.Name));
            }   
        }


        /// <summary>
        ///     Обрабатывает все входящие сообщения и
        ///     перенаправляет их в нужные места
        /// </summary>
        public void HandleMessage(string data, RAOConnection connection) {
//            Log.Debug("Recieved: " + data);
            try{
                var json = JToken.Parse(data);

                // Проверка на наличие необходимых ключей
                if (MsgDict.ClientRequestKeys.Any(key=>json[key] == null)){
                    throw new InvalidDataFormat();
                }

                // Без логина можно посылать только запрос на подключение
                if (connection.Player == null && json["type"].ToString() != MsgDict.ClientConnect){
                    throw new NotLoggedIn();
                }

                // Проверка валидности значений
                if (MsgDict.AviableApi.All(api=>api != json["api"].ToString())){
                    throw new InvalidApiVersion();
                }
                if (MsgDict.ClientTypeValues.All(type=>type != json["type"].ToString())){
                    throw new InvalidDataValues();
                }

                var jsonData = JToken.Parse(json["data"].ToString());

                switch (json["type"].ToString()){
                    case MsgDict.ClientConnect:
                        _handleConnect(connection, jsonData);
                        break;
                    case MsgDict.ClientConnectRoom:
                        _handleConnectRoom(connection, jsonData);
                        break;
                    case MsgDict.ClientStatus:
                        _handleStatus(connection, jsonData);
                        break;
                    case MsgDict.ClientRequest:
                        _handleRequest(connection, jsonData);
                        break;
                    case MsgDict.ClientDisconnect:
                        _handleDisconnect(connection, jsonData);
                        break;
                    case MsgDict.ClientControl:
                        _handleControl(connection, jsonData);
                        break;
                    case MsgDict.ClientChat:
                        _handleChat(connection, jsonData);
                        break;
                }
            }
            catch (Exception ex){
                if (ex is JsonReaderException){
                    connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeIncorrectDataFormat));
                }
                else if (ex is RAOException){
                    connection.SendData(ServerMessage.ResponseCode((ex as RAOException).Code));
                }
                else{
                    _handleError(connection, ex);
                }
            }
        }


        private void _handleConnect(RAOConnection connection, JToken jsonData) {
            if (connection.Player != null){
                throw new AlreadyLogged();
            }
            if (MsgDict.ClientConnectKeys.Any(key=>jsonData[key] == null)){
                throw new InvalidDataFormat();
            }

            var login = jsonData["login"].ToString();
            var password = jsonData["password"].ToString();

            if (login == "" && password == ""){
                var player = RegisterPlayer(connection.ID, "InsalityTEST" + new Random().Next(10, 99), connection);
                connection.Player = player;
                connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeSuccessful));
            }
            else{
                throw new InvalidLoginOrPassword();
            }
        }

        private void _handleDisconnect(RAOConnection connection, JToken jsonData) {
            if (connection.Player.CurrentRoom != null){
                connection.Player.CurrentRoom.DisconnectPlayer(connection.Player);
            }
            else if (connection.Player != null){
                RemovePlayer(connection.ID);
                connection.CloseConnection("Disconnect by user");
            }
        }

        private void _handleConnectRoom(RAOConnection connection, JToken jsonData) {
            if (jsonData["index"] == null){
                throw new InvalidDataFormat();
            }
            if (connection.Player.CurrentRoom != null){
                throw new PlayerNotInLobby();
            }

            var roomIndex = jsonData["index"];
            RAORoom rm;
            try{
                rm = _serverRooms.Find(room=>room.Id == int.Parse(roomIndex.ToString()));
            }
            catch (FormatException){
                throw new InvalidDataValues();
            }

            if (rm != null){
                rm.ConnectPlayer(connection.Player);
                connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeSuccessful));
            }
            else{
                throw new InvalidDataValues();
            }
        }

        private void _handleRequest(RAOConnection connection, JToken jsonData) {
            if (connection.Player.State != PlayerStates.PlayerGame){
                throw new PlayerNotInGame();
            }
            if (jsonData["requests"] == null){
                throw new InvalidDataFormat();
            }

            var sm = new ServerMessage {Code = MsgDict.CodeSuccessful, Type = MsgDict.ServerInformation};

            var data = new JObject();
            var requests = jsonData["requests"];

            // Game Requests:
            if (requests.ToList().Contains("map")){
                data.Add("map", connection.Player.CurrentRoom.GetStringMap());
            }

            if (requests.ToList().Contains("players")){
                data.Add("players", connection.Player.CurrentRoom.GetPlayersInfo());
            }

            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');
            connection.SendData(sm.Serialize());
        }

        private void _handleStatus(RAOConnection connection, JToken jsonData) {
            var sm = new ServerMessage {Code = 200, Type = MsgDict.ServerStatus};

            if (jsonData["requests"] == null){
                throw new InvalidDataFormat();
            }

            var data = new JObject();
            var requests = jsonData["requests"];

            // Lobby && Any requests:
            if (requests.ToList().Contains("roomlist")){
                var roomlist = GetRooms().Select(room=>room.GetInfo()).ToList();
                data.Add("roomlist", JToken.FromObject(roomlist));
            }

            if (requests.ToList().Contains("player")){
                data.Add("player", connection.Player.GetInfo());
            }

            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');
            connection.SendData(sm.Serialize());
        }

        private void _handleChat(RAOConnection connection, JToken jsonData) {
            if (jsonData["message"] == null){
                throw new InvalidDataFormat();
            }
            if (jsonData["message"].ToString() == ""){
                throw new InvalidDataValues();
            }

            if (connection.Player.State == PlayerStates.PlayerGame){
                connection.Player.CurrentRoom.ChatToRoom(jsonData["message"].ToString(), connection.Player.Name);
            }
            else{
                ChatToLobby(jsonData["message"].ToString(), connection.Player.Name);
            }
        }

        public void ChatToLobby(string msg, string sender) {
            msg = string.Format("[Lobby] {0}: {1}", sender, msg);
            Log.Game(msg);

            var sm = new ServerMessage {Code = MsgDict.CodeSuccessful, Type = MsgDict.ServerInformation};
            var data = new JObject {{"chat", msg}};
            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');

            foreach (var pl in _allPlayers.Where(pl=>pl.State == PlayerStates.PlayerLobby)){
                pl.Connection.SendData(sm.Serialize());
            }
        }

        private void _handleControl(RAOConnection connection, JToken jsonData) {
            if (connection.Player.State != PlayerStates.PlayerGame){
                throw new PlayerNotInGame();
            }
            if (jsonData["action"] == null){
                throw new InvalidDataFormat();
            }

            if (!MsgDict.ClientControlCommands.Contains(jsonData["action"].ToString())){
                throw new InvalidDataValues();
            }

            connection.Player.Hero.ActionQueue(jsonData["action"].ToString());
            connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeSuccessful));
        }

        private void _handleError(RAOConnection connection, Exception ex) {
            connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeServerError));
            Log.Error("Got exception in Handle message: " + ex);
        }
    }
}