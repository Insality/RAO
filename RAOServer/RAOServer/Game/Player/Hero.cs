using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game.Player {
    internal class Hero {
        public Stat Damage;
        public Stat Endurance;
        public Stat Health;
        public int Level;
        public char Sym;
        private RAORoom _room;

        public int X;
        public int Y;

        public Hero(RAORoom room) {
            Damage = new Stat(10);
            Endurance = new Stat(50);
            Health = new Stat(100);
            _room = room;

            Level = 1;
            Sym = '@';
        }

        public void Action(string action) {
            switch (action){
                case "control_up":
                    if (!_room.GetTiles()[Y-1][X].IsSolid){
                        Y--;
                    }
                    break;
                case "control_down":
                    if (!_room.GetTiles()[Y + 1][X].IsSolid) {
                        Y++;
                    }
                    break;
                case "control_left":
                    if (!_room.GetTiles()[Y][X-1].IsSolid) {
                        X--;
                    }
                    break;
                case "control_right":
                    if (!_room.GetTiles()[Y][X+1].IsSolid) {
                        X++;
                    }
                    break;
            }
        }

        public JToken GetInfo() {
            var info = new JObject {
                {"Damage", Damage.Current},
                {"Health", Health.Current},
                {"Level", Level},
                {"Sym", Sym},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }
    }
}