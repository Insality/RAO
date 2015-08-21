using System.Linq;
using RAOServer.Game.Entities.Enemies;
using RAOServer.Utils;

namespace RAOServer.Game.Entities.Enviroment {
    internal class Spawn: Entity {
        private Enemy SpawnedEnemy;
        private int _spawnCooldown;
        private int _curSpawnCooldown;

        public Spawn(int x, int y, RAORoom room): base(x, y, "Spawn", "Spawn", 20, 0, EntityType.Enemy, room) {
            IsSolid = true;
            SpawnedEnemy = null;
            _spawnCooldown = 10;
            _curSpawnCooldown = _spawnCooldown;
        }

        public override void Action(Entity source) {
            if (source.EntityType == EntityType.Player){
                Health -= source.Damage.Current;
            }
        }


        public override void Update() {
            base.Update();
            if (SpawnedEnemy == null){
                _curSpawnCooldown--;
            }

            if (_curSpawnCooldown <= 0){
                _curSpawnCooldown = 10;
                SpawnedEnemy = new Enemy(X, Y+1, Room);
                Room.Entities.Add(SpawnedEnemy);
            }

            if (SpawnedEnemy != null){
                if (SpawnedEnemy.Health.Current <= 0){
                    SpawnedEnemy = null;
                }
            }
        }
    }
}