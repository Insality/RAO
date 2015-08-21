using Newtonsoft.Json.Linq;

namespace RAOServer.Game.Entities.Enviroment {
    internal class PressurePlate: Entity {
        public bool IsPressed;

        public PressurePlate(int x, int y, RAORoom room): base(x, y, "PressurePlate", "PressurePlate", 10, 0, EntityType.Item, room) {
            IsSolid = false;
            IsPressed = false;
        }

        public override void Action(Entity source) {
            if (source.EntityType == EntityType.Player) {
                IsPressed = true;
                Image = "PressurePlateOn";
            }
        }
    }
}