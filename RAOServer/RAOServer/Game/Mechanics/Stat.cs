using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAOServer.Game.Mechanics {
    class Stat {
        public int Max;
        public int Current;

        public Stat(int max) {
            Max = max;
            Current = Max;
        }

        // TODO: переопределить манипуляцию со статами
    }
}
