using RAOServer.Game.Player;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class RAOConnection: WebSocketBehavior {
        private readonly RAOServer _server;
        public Player Player;
        public string SessionId;

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
            base.OnMessage(e);
            _server.HandleMessage(e.Data, this);
        }

        protected override void OnOpen() {
            base.OnOpen();
            Utils.Log.Network("Client connected from " + Context.UserEndPoint);
            SessionId = ID;
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            Utils.Log.Error("CLOSE ID: " + ID);
            _server.RemovePlayer(ID);
        }

        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            Utils.Log.Error("ERROR IN ID: " + ID);
        }
    }
}