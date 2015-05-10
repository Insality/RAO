using System;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;
using RAOServer.Utils;

namespace RAOServer.Game.PlayerStuff {
    internal class Hero:Entity {
        private readonly RAORoom _room;
        public Stat Endurance;
        public Stat Initiative;
        public int Level;
        public char Sym;
        public Hero(RAORoom room): base("player", "player") {
            Damage = new Stat(10);
            Endurance = new Stat(50);
            Health = new Stat(100);
            Initiative = new Stat(new Random().Next(1, 20));
            _room = room;

            Level = 1;
            Sym = '@';
        }

        public override void Action() {
            base.Action();
            switch (LastAction){
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
            LastAction = "";
        }

        public override JObject GetInfo() {
            var info = base.GetInfo();

            info.Add("Level", Level);
            info.Add("Initiative", Initiative.Current);
            return info;
        }
    }
}