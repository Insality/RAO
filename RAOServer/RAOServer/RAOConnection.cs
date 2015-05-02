using RAOServer.Game.Player;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOConnection: WebSocketBehavior {
        private readonly RAOServer _server;
        public string SessionId;
        public Player Player;

        public RAOConnection() {
            _server = RAOServer.Instance;
        }

        public void SendData(string data) {
            Send(data);
        }

        protected override void OnMessage(MessageEventArgs e) {
            _server.HandleMessage(e.Data, this);
        }

        protected override void OnOpen() {
            Log.Network("Client connected from " + Context.UserEndPoint);
            SessionId = ID;
            Player = _server.RegisterPlayer(SessionId);
        }

        protected override void OnClose(CloseEventArgs e) {
            // TODO: не отрабатывает
            Log.Network("Client dsconnected: " + Context.UserEndPoint + ". Reason: " + e);
            Send("CLOSE");
            _server.RemovePlayer(SessionId);
        }
    }
}