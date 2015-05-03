using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAOServer.Game;
using RAOServer.Game.Player;
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
            timer.Enabled = true;
            GC.KeepAlive(timer);
        }

        public static RAOServer Instance {
            get { return _instance ?? (_instance = new RAOServer()); }
        }

        public void Start() {
            // Init all services
            _webSocketServer = new WebSocketServer(string.Format("ws://{0}:{1}", Settings.Ip, Settings.Port));
            _webSocketServer.AddWebSocketService<RAOConnection>(Settings.GameRoute);

            _serverConnections = new Thread(ServerConnectionsHandler);
            _serverConnections.Start();

            _serverConsole = new Thread(ServerConsoleHandler);
            _serverConsole.Start();

            CreateNewRoom();
        }

        public int CreateNewRoom() {
            var newRoom = new RAORoom(this);
            _serverRooms.Add(newRoom);
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
            foreach (Player player in _allPlayers.ToList()){
                if (!_webSocketServer.WebSocketServices[Settings.GameRoute].Sessions.IDs.Contains(player.Id)){
                    RemovePlayer(player.Id);
                }
            }
        }

        public Player RegisterPlayer(string id, string login) {
            var pl = new Player(id, login);
            _allPlayers.Add(pl);

            Log.Game(string.Format("Player {0} joined to the game", pl.Name));

            return pl;
        }

        public void RemovePlayer(string id) {
            Player pl = _allPlayers.Find(player=>player.Id == id);
            _allPlayers.Remove(pl);

            Log.Game(string.Format("Player {0} has disconnected", pl.Name));
        }


        /// <summary>
        ///     Обрабатывает все входящие сообщения и
        ///     перенаправляет их в нужные места
        /// </summary>
        public void HandleMessage(string data, RAOConnection connection) {
            try{
                JToken json = JToken.Parse(data);

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

                JToken jsonData = JToken.Parse(json["data"].ToString());

                if (connection.Player == null)
                    Log.Debug(string.Format("{2} Msg from {0}: {1}", connection.ID, json["data"], json["type"]));
                else
                    Log.Debug(string.Format("{2} Msg from {0}: {1}", connection.Player.Name, json["data"], json["type"]));

                switch (json["type"].ToString()){
                    case MsgDict.ClientConnect:
                        HandleConnect(connection, json, jsonData);
                        break;
                    case MsgDict.ClientConnectRoom:
                        HandleConnectRoom(connection, json, jsonData);
                        break;
                    case MsgDict.ClientStatus:
                        break;
                    case MsgDict.ClientRequest:
                        HandleRequest(connection, json, jsonData);
                        break;
                    case MsgDict.ClientDisconnect:
                        if (connection.Player != null)
                            RemovePlayer(connection.ID);
                        connection.CloseConnection("Disconnect by user");
                        break;
                    case MsgDict.ClientControl:
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
                    Log.Error("Got exception in Handle message: " + ex);
                }
            }
        }


        private void HandleConnect(RAOConnection connection, JToken json, JToken jsonData) {
            if (connection.Player != null){
                throw new AlreadyLogged();
            }
            if (MsgDict.ClientConnectKeys.Any(key=>jsonData[key] == null)){
                throw new InvalidDataFormat();
            }

            string login = jsonData["login"].ToString();
            string password = jsonData["password"].ToString();

            if (login == "" && password == ""){
                Player player = RegisterPlayer(connection.ID, login);
                connection.Player = player;
                connection.SendData(ServerMessage.ResponseCode(MsgDict.CodeSuccessful));
            }
            else{
                throw new InvalidLoginOrPassword();
            }
        }

        private void HandleConnectRoom(RAOConnection connection, JToken json, JToken jsonData) {
            if (jsonData["index"] == null){
                throw new InvalidDataFormat();
            }
            if (connection.Player.CurrentRoom != -1){
                throw new PlayerNotInLobby();
            }

            JToken roomIndex = jsonData["index"];
            RAORoom rm = _serverRooms.Find(room=>room.Id == int.Parse(roomIndex.ToString()));

            if (rm != null){
                rm.ConnectPlayer(connection);
            }
            else{
                throw new InvalidDataValues();
            }
        }

        private void HandleRequest(RAOConnection connection, JToken json, JToken jsonData) {
            if (jsonData["requests"] == null){
                throw new InvalidDataFormat();
            }

            var sm = new ServerMessage();
            sm.Code = 200;
            sm.Type = MsgDict.ServerInformation;
            sm.Error = MsgDict.MsgCode[sm.Code];

            var data = new JObject();
            JToken requests = jsonData["requests"];

            if (requests.ToList().Contains("roomlist")){
                var roomlist = new List<JObject>();
                foreach (RAORoom room in GetRooms()){
                    roomlist.Add(room.GetInfo());
                }
                data.Add("roomlist", JToken.FromObject(roomlist));
            }

            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');
            connection.SendData(sm.Serialize());
        }
    }
}