using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;
using RAOServer.Utils;

namespace RAOServer.Game {
    internal abstract class Entity {
        public Stat Damage;
        public Stat Health;

        public string Image;

        protected string LastAction = "";
        public string Name;
        protected RAORoom Room;
        public int X;
        public int Y;

        protected Entity(string image, string name, int health, int damage, RAORoom room) {
            X = 0;
            Y = 0;
            Health = new Stat(health);
            Damage = new Stat(damage);
            Room = room;
            Image = image;
            Name = name;
        }

        public void ActionQueue(string action) {
            LastAction = action;
        }

        public virtual void Action() {
            switch (LastAction) {
                case "control_up":
                    ActionBy(0, -1);
                    break;
                case "control_down":
                    ActionBy(0, 1);
                    break;
                case "control_left":
                    ActionBy(-1, 0);
                    break;
                case "control_right":
                    ActionBy(1, 0);
                    break;
            }
            LastAction = "";
        }

        public virtual JObject GetInfo() {
            var info = new JObject {
                {"Damage", Damage.Current},
                {"Health", Health.Current},
                {"HealthMax", Health.Max},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }

        public void MoveBy(int x, int y) {
            if (Room.GetTiles()[Y + y][X + x].IsSolid) return;
            Y += y;
            X += x;
        }

        public void ActionBy(int x, int y) {
            var entity = Room.GetEntity(X + x, Y + y);
            if (entity != null){
                Action(entity);
            }
            else{
                MoveBy(x, y);
            }
        }

        public void Action(Entity other) {
            // This doing action on other
            // now just a damage
            other.Health.Current -= Damage.Current;
        }
    }
}