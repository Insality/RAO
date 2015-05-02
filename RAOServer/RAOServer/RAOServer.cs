using System.Collections.Generic;
using System.Threading;
using RAOServer.Game;
using RAOServer.Game.Player;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOServer: WebSocketBehavior {
        private Thread _serverConnections;
        private Thread _serverConsole;
        private WebSocketServer _webSocketServer;
        // TODO: Сделать не статичными, разобраться с веб-сокетом.. :(
        private static List<RAORoom> _serverRooms;
        private static List<Player> _allPlayers;
        private string test = "STESDAS";

        public RAOServer() {

            Log.Debug("Rao Server constructor");
        }

        public void Start() {
            // Init all lists
            _serverRooms = new List<RAORoom>();
            _allPlayers = new List<Player>();
            // Init all services
            _webSocketServer = new WebSocketServer(string.Format("ws://{0}:{1}", Settings.Ip, Settings.Port));
            _webSocketServer.AddWebSocketService<RAOServer>(Settings.GameRoute);

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

        protected override void OnMessage(MessageEventArgs e) {
            string msg = e.Data == "BALUS"
                ? "I've been balused already..."
                : "I'm not available now.";
            Log.Debug("Got message from ???. Data: " + e.Data);
            Send(msg);
        }

        protected override void OnOpen() {
            Log.Network("Client connected from " + Context.UserEndPoint);
            RegisterPlayer(Context.UserEndPoint.Port);
            Log.Debug(test);
        }

        private void RegisterPlayer(int Id) {
            var player = new Player { Id = Id };
            _allPlayers.Add(player);
            Log.Terminal("COUNT " + _allPlayers.Count);
        }

        private void RemovePlayer(int Id) {
            _allPlayers.Remove(_allPlayers.Find(player=>player.Id == Id));
        }

        protected override void OnClose(CloseEventArgs e) {
            // TODO: не отрабатывает
            Log.Network("Client dsconnected: " + Context.UserEndPoint + ". Reason: " + e);
            RemovePlayer(Context.UserEndPoint.Port);
        }
    }
}