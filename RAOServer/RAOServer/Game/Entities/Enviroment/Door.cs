using System.Linq;
using RAOServer.Utils;

namespace RAOServer.Game.Entities.Enviroment {
    internal class Door: Entity {
        public bool IsClosed;

        public Door(int x, int y, RAORoom room): base(x, y, "DoorClosed", "Door", 10, 0, EntityType.Item, room) {
            IsSolid = true;
            IsClosed = true;
        }

        public override void Action(Entity source) {
            if (source.Name == "Player"){
                if (IsClosed){
                    var plates = Room.Entities.Where(ent=>ent is PressurePlate && !(ent as PressurePlate).IsPressed);
                    if (!plates.Any()){
                        IsSolid = false;
                        IsClosed = false;
                        Image = "DoorOpened";
                    }
                }
            }
        }
    }
}