using System;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;
using RAOServer.Utils;

namespace RAOServer.Game.PlayerStuff {
    internal class Hero:Entity {
        public Stat Endurance;
        public Stat Initiative;
        public int Level;
        public char Sym;
        public Hero(RAORoom room): base("player", "player", 10, 2, room) {
            Endurance = new Stat(50);
            Initiative = new Stat(new Random().Next(1, 20));

            Level = 1;
            Sym = '@';
        }

        public override JObject GetInfo() {
            var info = base.GetInfo();

            info.Add("Level", Level);
            info.Add("Initiative", Initiative.Current);
            return info;
        }
    }
}