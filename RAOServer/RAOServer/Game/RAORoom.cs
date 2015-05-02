using System;
using System.Collections.Generic;
using RAOServer.Utils;

namespace RAOServer.Game {
    /// <summary>
    ///     RAORoom - игровая комната одной арены. Создается при создании лобби
    ///     и обрабатывает все действия в этой комнате
    /// </summary>
    internal class RAORoom {
        private readonly Map _map = new Map();
        private static int _roomCounter = 0;
        public int Id;
        private List<Player.Player> _players;
        private RAOServer _server;

        public RAORoom(RAOServer server) {
            Id = _roomCounter++;
            _players = new List<Player.Player>();
            _server = server;
            _map.LoadMapFromFile("testMap.txt");
        }

        public void PrintMap() {
            foreach (var tile in _map.Tiles){
                Log.Debug(String.Join("", tile));
            }
        }

        public string GetStringMap() {
            var str = "";
            foreach (var tileRow in _map.Tiles) {
                str += string.Join("", tileRow);
                str += '\n';
            }
            return str;
        }
    }
}