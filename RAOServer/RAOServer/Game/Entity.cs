using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game {
    internal abstract class Entity {
        public string Name;
        public int X;
        public int Y;
        public Stat Health;
        public Stat Damage;

        public string Image;

        protected string LastAction = "";

        protected Entity(string image, string name) {
            X = 0;
            Y = 0;
            Image = image;
            Name = name;
        }

        public void ActionQueue(string action) {
            LastAction = action;
        }

        public virtual void Action() {
        }

        public virtual JObject GetInfo() {
            var info = new JObject {
                {"Damage", Damage.Current},
                {"Health", Health.Current},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }
    }
}