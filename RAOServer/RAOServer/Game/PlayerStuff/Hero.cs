using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game.PlayerStuff {
    internal class Hero: Entity {
        public Stat Endurance;
        public List<Tile> FOV;
        public Stat Initiative;
        public int Level;
        public Stat SeeRadius;
        public char Sym;

        public Hero(int x, int y, RAORoom room): base(x, y, "Player", "Player", 10, 2, EntityType.Player, room) {
            Endurance = new Stat(50);
            SeeRadius = new Stat(10);

            Level = 1;
            Sym = '@';
        }

        public override JObject GetInfo() {
            var info = base.GetInfo();

            info.Add("Level", Level);
            
            return info;
        }

        public override void Update() {
            base.Update();
            FOV = Room.GetMap().GetFov(this);
        }

        public override void Action(Entity source) {
            if (source.EntityType == EntityType.Player || source.EntityType == EntityType.Enemy){
                Health -= source.Damage.Current;
            }
        }
    }
}