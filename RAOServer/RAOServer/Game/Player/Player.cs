namespace RAOServer.Game.Player {
    internal class Player {
        public string Id;
        public string Name;
        public Hero Hero;
        public int Level;
        public PlayerStatistic Statistic;
        public int CurrentRoom;


        public Player(string Id, string Name) {
            this.Id = Id;
            this.Name = Name;
            CurrentRoom = -1;
        }

        public override string ToString() {
            return Name + ":" + Id;
        }
    }
}