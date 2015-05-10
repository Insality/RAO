using Newtonsoft.Json.Linq;

namespace RAOServer.Game.Entities.Enviroment {
    internal class PressurePlate: Entity {
        public bool IsPressed;

        public PressurePlate(RAORoom room): base("PressurePlate", "PressurePlate", 10, 0, room) {
            IsSolid = false;
            IsPressed = false;
        }

//        public override JObject GetInfo() {
//            var info = base.GetInfo();
//            info.Add("IsPressed", IsPressed);
//            return info;
//        }

        public override void Action(Entity source) {
            if (source.Name == "Player"){
                IsPressed = true;
                Image = "PressurePlateOn";
            }
        }
    }
}