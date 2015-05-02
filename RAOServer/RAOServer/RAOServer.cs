using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAOServer.Game;
using RAOServer.Game.Player;
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
        /// Обрабатывает все входящие сообщения и 
        /// перенаправляет их в нужные места
        /// </summary>
        public void HandleMessage(string data, RAOConnection connection) {
            try{
                var json = JToken.Parse(data);
                Log.Debug(string.Format("Msg from {0}: {1}", connection.Player.Name, json["Test"]));

                if (json["GetId"].ToString() == "My"){
                    connection.SendData(connection.ID);
                }
            }
            catch (JsonReaderException jex){
                connection.SendData("{'error': 'Wrong json format'}");
            }
            catch (Exception ex){
                Log.Error("Got exception in Handle message: " + ex.ToString());
            }
        }
    }
}