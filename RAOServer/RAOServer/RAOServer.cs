using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RAOServer.Game;
using RAOServer.Game.Player;
using RAOServer.Utils;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOServer {
        private static RAOServer _instance;

        private static List<RAORoom> _serverRooms;
        private static List<Player> _allPlayers;
        private Thread _serverConnections;
        private Thread _serverConsole;
        private WebSocketServer _webSocketServer;

        private RAOServer() {
            // Init all lists
            _serverRooms = new List<RAORoom>();
            _allPlayers = new List<Player>();
        }

        public static RAOServer Instance {
            get { return _instance ?? (_instance = new RAOServer()); }
        }

        public void Start() {
            // Init all services
            _webSocketServer = new WebSocketServer(string.Format("ws://{0}:{1}", Settings.Ip, Settings.Port));
            _webSocketServer.AddWebSocketService<RAOConnection>(Settings.GameRoute);

            _serverConnections = new Thread(ServerHandleConnections);
            _serverConnections.Start();

            _serverConsole = new Thread(ServerHandleConsole);
            _serverConsole.Start();

            _serverConnections.Join();
            _serverConsole.Join();
        }

        public int CreateNewRoom() {
            var newRoom = new RAORoom(this);
            _serverRooms.Add(newRoom);
            return newRoom.Id;
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

        private void ServerHandleConsole(object obj) {
            var serverConsole = new ServerConsole(this);
            serverConsole.Start();
        }

        private void ServerHandleConnections(object obj) {
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

        public void RegisterPlayer(string id) {
            var player = new Player {Id = id};
            _allPlayers.Add(player);
            Log.Terminal("COUNT " + _allPlayers.Count);
        }

        public void RemovePlayer(string id) {
            Player pl = _allPlayers.Find(player=>player.Id == id);
            Log.Game(string.Format("Player {0} has disconnected", pl.Name));
            _allPlayers.Remove(pl);
        }
    }
}