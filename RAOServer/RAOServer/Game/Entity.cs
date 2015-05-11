using System.Linq;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game {
    internal abstract class Entity {
        private static int _idCounter;
        public Stat Damage;
        public Stat Health;
        public int Id;
        // Можно ли проходить через сущность?

        public string Image;
        public bool IsSolid;

        protected string LastAction = "";
        public string Name;
        protected RAORoom Room;
        public int X;
        public int Y;

        protected Entity(int x, int y, string image, string name, int health, int damage, RAORoom room) {
            Id = _idCounter;
            _idCounter++;
            X = x;
            Y = y;
            Health = new Stat(health);
            Damage = new Stat(damage);
            Room = room;
            Image = image;
            Name = name;
            IsSolid = true;
        }

        public void ActionQueue(string action) {
            LastAction = action;
        }

        public virtual void Action() {
            switch (LastAction){
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
                case "control_action":
                    ActionBy(0, 0);
                    break;
            }
            LastAction = "";
        }

        public virtual JObject GetInfo() {
            var info = new JObject {
                {"Id", Id},
                {"Damage", Damage.Current},
                {"Health", Health.Current},
                {"HealthMax", Health.Max},
                {"Name", Name},
                {"Image", Image},
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
            var entities = Room.GetEntities(X + x, Y + y);
            // в случае, если на клетке и активируемый предмет и существо, выбираем только существ
            if (entities.Any(e=>e.IsSolid && e != this)){
                entities = entities.Where(e=>e.IsSolid && e != this).ToList();
            }

            foreach (var entity in entities){
                if (entity != this && (entity.IsSolid || (x == 0 && y == 0))){
                    entity.Action(this);
                }
                else{
                    MoveBy(x, y);
                }
            }

            if (!entities.Any())
                MoveBy(x, y);
        }

        public virtual void Action(Entity source) {
        }
    }
}