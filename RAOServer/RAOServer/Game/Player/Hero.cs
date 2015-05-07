using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game.Player {
    internal class Hero {
        public Stat Damage;
        public Stat Endurance;
        public Stat Health;
        public int Level;
        public char Sym;

        public Hero() {
            Damage = new Stat(10);
            Endurance = new Stat(50);
            Health = new Stat(100);

            Level = 1;
            Sym = '@';
        }

        public JToken GetInfo() {
            var info = new JObject { { "Damage", Damage.Current}, { "Health", Health.Current }, { "Level", Level }, { "Sym", Sym } };
            return info;
        }
    }
}