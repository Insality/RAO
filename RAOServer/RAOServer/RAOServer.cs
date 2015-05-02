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
using WebSocketSharp;
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

        public Player RegisterPlayer(string id) {
            var pl = new Player {Id = id};
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

                // Проверка валидности значений
                if (MsgDict.AviableApi.All(api=>api != json["api"].ToString())){
                    throw new InvalidApiVersion();
                }
                if (MsgDict.ClientTypeValues.All(type=>type != json["type"].ToString())){
                    throw new InvalidDataValues();
                }

                JToken jsonData = JToken.Parse(json["data"].ToString());

                Log.Debug(string.Format("{2} Msg from {0}: {1}", connection.Player.Name, json["data"], json["type"]));

                switch (json["type"].ToString()){
                    case MsgDict.ClientConnect:
                        break;
                    case MsgDict.ClientConnectRoom:
                        break;
                    case MsgDict.ClientStatus:
                        break;
                    case MsgDict.ClientRequest:
                        break;
                    case MsgDict.ClientDisconnect:
                        RemovePlayer(connection.ID);
                        _webSocketServer.WebSocketServices[Settings.GameRoute].Sessions.CloseSession(connection.ID,
                            CloseStatusCode.Normal, "Disconnect by user"
                            );
                        break;
                    case MsgDict.ClientControl:
                        break;
                }
            }
            catch (Exception ex){
                if (ex is JsonReaderException || ex is InvalidDataFormat){
                    connection.SendData(ServerMessage.ResponseError(MsgDict.CodeIncorrectDataFormat));
                }
                else if (ex is InvalidDataValues){
                    connection.SendData(ServerMessage.ResponseError(MsgDict.CodeIncorrectDataValues));
                }
                else if (ex is InvalidApiVersion){
                    connection.SendData(ServerMessage.ResponseError(MsgDict.CodeIncorrectApiVersion));
                }
                else{
                    Log.Error("Got exception in Handle message: " + ex);
                }
            }
        }
    }
}