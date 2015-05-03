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

        public void CloseConnection(string message) {
           Sessions.CloseSession(ID, CloseStatusCode.Normal, message);
        }

        protected override void OnMessage(MessageEventArgs e) {
            _server.HandleMessage(e.Data, this);
        }

        protected override void OnOpen() {
            Log.Network("Client connected from " + Context.UserEndPoint);
            SessionId = ID;
        }
    }
}