using System.Net;
using System.Net.Sockets;
using System.Threading;
using RAOServer.Game;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOServer: WebSocketBehavior {
        private WebSocketServer _webSocketServer;
        private Thread _serverConnections;
        private Thread _serverConsole;
        private Thread _serverRoom;

        public void Start() {
            _webSocketServer = new WebSocketServer(string.Format("ws://{0}:{1}", Settings.Ip, Settings.Port));
            _webSocketServer.AddWebSocketService<RAOServer>(Settings.GameRoute);

            _serverConnections = new Thread(ServerHandleConnections);
            _serverConnections.Start();

            _serverConsole = new Thread(ServerHandleConsole);
            _serverConsole.Start();

            _serverRoom = new Thread(ServerRoomHandler);
            _serverRoom.Start();
        }

        private void ServerHandleConsole(object obj) {
            var serverConsole = new ServerConsole(_webSocketServer);
            serverConsole.Start();
        }

        private void ServerRoomHandler(object obj) {
            var room = new RAORoom();
            room.PrintMap();
        }

        private void ServerHandleConnections(object obj) {
            Log.Network("Starting Listen connections on " + Settings.Ip + ":" + Settings.Port);

            // For the information about this: https://github.com/sta/websocket-sharp           
            _webSocketServer.Start();

            Log.Network("Waiting for connections");
        }

        protected override void OnMessage(MessageEventArgs e) {
            var msg = e.Data == "BALUS"
                      ? "I've been balused already..."
                      : "I'm not available now.";
            Log.Debug("Got message from ???. Data: " + e.Data);
            Send(msg);
        }

        protected override void OnOpen() {
            Log.Network("Client connected from " + Context.UserEndPoint );
        }

        protected override void OnClose(CloseEventArgs e) {
             Log.Network("Client dsconnected: " + Context.UserEndPoint + ". Reason: " + e.ToString());
        }

    }
}