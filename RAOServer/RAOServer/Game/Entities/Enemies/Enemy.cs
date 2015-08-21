using System;
using System.Collections.Generic;

namespace RAOServer.Game.Entities.Enemies {
    internal class Enemy: Entity {
        private readonly List<String> _patternList;
        private int _curStep;

        public Enemy(int x, int y, RAORoom room): base(x, y, "Enemy", "Enemy", 9, 2, EntityType.Enemy, room) {
            _curStep = 0;
            _patternList = new List<string> { "control_up", "", "control_right", "", "control_down", "", "control_left", ""};
            Initiative.Max = 1;
        }

        public override void Update() {
            base.Update();
            UpdatePattern();
        }

        private void UpdatePattern() {
            ActionQueue(_patternList[_curStep++]);
            Action();

            if (_curStep >= _patternList.Count){
                _curStep = 0;
            }
        }

        public override void Action(Entity source) {
            base.Action();
            if (source.EntityType == EntityType.Player) {
                Health -= source.Damage.Current;
            }
        }
    }
}