using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RAOServer.Utils;

namespace RAOServer.Game {
    /// <summary>
    /// RAORoom - игровая комната одной арены. Создается при создании лобби
    /// и обрабатывает все действия в этой комнате
    /// </summary>
    class RAORoom {
        private Map map = new Map();

        public RAORoom() {
            map.LoadMapFromFile("testMap.txt");
        }

        public void PrintMap() {
            foreach (var tile in map.tiles){
                Log.Debug(String.Join("", tile));
            }
        }
    }
}
