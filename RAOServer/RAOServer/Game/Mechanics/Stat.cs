namespace RAOServer.Game.Mechanics {
    internal class Stat {
        public int Current;
        public int Max;

        public Stat(int max) {
            Max = max;
            Current = Max;
        }

        // TODO: переопределить манипуляцию со статами
    }
}