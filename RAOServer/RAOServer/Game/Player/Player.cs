namespace RAOServer.Game.Player {
    internal class Player {
        public string Id;
        public string Name = "unnamed";
        public Hero Hero;
        public int Level;
        public PlayerStatistic Statistic;
        public int CurrentRoom;


        public override string ToString() {
            return Name + ":" + Id;
        }
    }
}