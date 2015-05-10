using System;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game.Player {
    internal class Hero {
        private readonly RAORoom _room;
        public Stat Damage;
        public Stat Endurance;
        public Stat Health;
        public Stat Initiative;
        public int Level;
        public char Sym;
        private string _lastAction;

        public int X;
        public int Y;

        public Hero(RAORoom room) {
            Damage = new Stat(10);
            Endurance = new Stat(50);
            Health = new Stat(100);
            Initiative = new Stat(new Random().Next(1, 20));
            _room = room;

            Level = 1;
            Sym = '@';
        }

        public void ActionQueue(string action) {
            _lastAction = action;
        }

        public void Action() {
            switch (_lastAction){
                case "control_up":
                    if (!_room.GetTiles()[Y - 1][X].IsSolid){
                        Y--;
                    }
                    break;
                case "control_down":
                    if (!_room.GetTiles()[Y + 1][X].IsSolid){
                        Y++;
                    }
                    break;
                case "control_left":
                    if (!_room.GetTiles()[Y][X - 1].IsSolid){
                        X--;
                    }
                    break;
                case "control_right":
                    if (!_room.GetTiles()[Y][X + 1].IsSolid){
                        X++;
                    }
                    break;
            }
            _lastAction = "";
        }

        public JToken GetInfo() {
            var info = new JObject {
                {"Damage", Damage.Current},
                {"Health", Health.Current},
                {"Level", Level},
                {"Sym", Sym},
                {"Initiative", Initiative.Current},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }
    }
}