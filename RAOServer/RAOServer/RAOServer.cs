using System.Net;
using System.Net.Sockets;
using System.Threading;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOServer: WebSocketBehavior {
//        private static NetServer _server;
        private static Thread _serverConnections;
        private static Thread _serverConsole;

        public void Start() {
            _serverConnections = new Thread(ServerHandleConnections);
            _serverConnections.Start();

            _serverConsole = new Thread(ServerHandleConsole);
            _serverConsole.Start();
        }

        private void ServerHandleConsole(object obj) {
            var serverConsole = new ServerConsole();
            serverConsole.Start();
        }

        private void ServerHandleConnections(object obj) {
            Log.Network("Starting Listen connections");

            // For the information about this: https://github.com/sta/websocket-sharp           
            var wssv = new WebSocketServer("ws://127.0.0.1:4080");
            wssv.AddWebSocketService<RAOServer>("/rao");
            wssv.Start();
            Log.Network("Waiting for connections");
//            wssv.Stop();
        }

        protected override void OnMessage(MessageEventArgs e) {
            var msg = e.Data == "BALUS"
                      ? "I've been balused already..."
                      : "I'm not available now.";
            Log.Debug("Gor message from ???. Data: " + e.Data);
            Send(msg);
        }

        protected override void OnOpen() {
            Log.Network("Client connected");
//            Log.Network(Context.Host);
//            Log.Network(Context.Headers.ToString());
//            Log.Network(Context.User.ToString());
//            Log.Network(Context.UserEndPoint.ToString());
        }
    }
}