using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOConnection: WebSocketBehavior {
        private readonly RAOServer _server;
        public string SessionId;

        public RAOConnection() {
            _server = RAOServer.Instance;
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
            SessionId = ID;
            _server.RegisterPlayer(SessionId);
        }

        protected override void OnClose(CloseEventArgs e) {
            // TODO: не отрабатывает
            Log.Network("Client dsconnected: " + Context.UserEndPoint + ". Reason: " + e);
            Send("CLOSE");
            _server.RemovePlayer(SessionId);
        }
    }
}