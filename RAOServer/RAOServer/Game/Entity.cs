using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Mechanics;

namespace RAOServer.Game {

    public enum EntityType {
        Player,
        Enemy, 
        Item
    }

    internal abstract class Entity {
        private static int _idCounter;
        public Stat Damage;
        public Stat Health;
        public Stat Initiative;
        public int Id;
        public string Image;
        // Можно ли проходить через сущность?
        public bool IsSolid;

        public string LastAction = "";
        public string Name;
        protected RAORoom Room;
        public EntityType EntityType; 
        public int X;
        public int Y;

        protected Entity(int x, int y, string image, string name, int health, int damage, EntityType type, RAORoom room) {
            Id = _idCounter;
            _idCounter++;
            X = x;
            Y = y;
            Health = new Stat(health);
            Damage = new Stat(damage);
            Initiative = new Stat(new Random().Next(1, 20));
            Room = room;
            EntityType = type;
            Image = image;
            Name = name;
            IsSolid = true;
        }

        public void ActionQueue(string action) {
            LastAction = action;
        }

        /// <summary>
        /// Выполняет действие, которое стоит в очереди на следующий ход
        /// </summary>
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
                {"Initiative", Initiative.Current},
                {"Name", Name},
                {"Image", Image},
                {"Type", EntityType.ToString()},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }

        public void MoveBy(int x, int y) {
            if (Room.GetTiles().Find(t=>t.X == X + x && t.Y == Y + y).IsSolid) return;
            Y += y;
            X += x;
        }

        public virtual void Update() {
            if (Health.Current <= 0){
                Room.KillEntity(this);
            }
        }

        public void ActionBy(int x, int y) {
            var entities = Room.GetEntities(X + x, Y + y);
            entities.Remove(this);
            // в случае, если на клетке и активируемый предмет и существо, выбираем только существ
            if (entities.Any(e=>e.IsSolid)){
                entities = entities.Where(e=>e.IsSolid).ToList();
            }

            foreach (var entity in entities){
                if (entity.IsSolid || (x == 0 && y == 0)){
                    entity.Action(this);
                }
                else{
                    entity.Action(this);
                    MoveBy(x, y);
                }
            }

            if (!entities.Any())
                MoveBy(x, y);
        }

        public double DistanceTo(Entity other) {
            return DistanceTo(other.X, other.Y);
        }

        public double DistanceTo(int x, int y) {
            return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
        }

        public virtual void Action(Entity source) {
        }
    }
}