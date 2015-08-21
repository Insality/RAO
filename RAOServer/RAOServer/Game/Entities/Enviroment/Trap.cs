using System.Linq;
using RAOServer.Utils;

namespace RAOServer.Game.Entities.Enviroment {
    internal class Trap: Entity {
        public bool IsActive;
        private int _trapCooldown;
        private int _curTrapCooldown;

        public Trap(int x, int y, RAORoom room): base(x, y, "TrapDeactive", "Tra", 10, 4, EntityType.Item, room) {
            IsSolid = false;
            IsActive = false;
            _trapCooldown = 12;
            _curTrapCooldown = 0;
        }

        public override void Action(Entity source) {
            if (source.EntityType == EntityType.Enemy || source.EntityType == EntityType.Player){
                if (!IsActive){
                    IsActive = true;
                    source.Health -= Damage.Current;
                    _curTrapCooldown = _trapCooldown;
                    Image = "TrapActive";
                }                
            }
        }

        public override void Update() {
            base.Update();
            if (IsActive) {
                _curTrapCooldown--;
                if (_curTrapCooldown <= 0) {
                    IsActive = false;
                    _curTrapCooldown = 0;
                    Image = "TrapDeactive";
                }
            }
        }
    }
}