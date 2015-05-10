using Newtonsoft.Json.Linq;
using RAOServer.Network;

namespace RAOServer.Game.Player {
    internal class Player {
        public RAORoom CurrentRoom;
        public Hero Hero;
        public string Id;
        public int Level;
        public string Name;
        public PlayerStates State;
        public PlayerStatistic Statistic;
        public RAOConnection Connection;

        private readonly RAOServer _server;


        public Player(string id, string name, RAOConnection connection) {
            Id = id;
            Name = name;
            CurrentRoom = null;
            State = PlayerStates.PlayerLobby;
            Hero = new Hero(null);
            _server = RAOServer.Instance;
            Connection = connection;
        }

        public override string ToString() {
            return Name + ":" + Id;
        }

        public JToken GetInfo() {
            var curRoomIndex = -1;
            if (CurrentRoom != null){
                curRoomIndex = CurrentRoom.Id;
            }
            var info = new JObject {{"Name", Name}, {"Hero", Hero.GetInfo()}, {"CurrentRoom",  curRoomIndex}, {"State", State.ToString()}};
            return info;
        }

        public void ConnectToRoom(int index) {
            if (State != PlayerStates.PlayerLobby){
                throw new PlayerNotInLobby();
            }

            CurrentRoom = _server.GetRoom(index);
            State = PlayerStates.PlayerGame;
        }

        public void ConnectToLobby() {
            if (State != PlayerStates.PlayerGame) {
                throw new PlayerNotInGame();
            }

            CurrentRoom = null;
            State = PlayerStates.PlayerLobby;
        }
    }
}