namespace RAOServer.Game.Mechanics {
    internal class Stat {
        private int _current;
        private int _max;

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

        public int Max {
            get { return _max; }
            set {
                if (value < Current){
                    Current = value;
                }
                _max = value;
            }
        }

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