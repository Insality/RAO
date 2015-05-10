namespace RAOServer.Game.Mechanics {
    internal class Stat {
        private int _current;

        public int Current {
            get { return _current; }
            set {
                if (value < 0){
                    _current = 0; }

                else if (value > Max){
                    _current = Max;
                }
                else _current = value;
            }
        }
        public int Max;

        public Stat(int max) {
            Max = max;
            Current = Max;
        }

        public static Stat operator +(Stat a, int b) {
            a.Current += b;
            return a;
        }

        public static Stat operator -(Stat a, int b) {
            a.Current -= b;
            return a;
        }
    }
}