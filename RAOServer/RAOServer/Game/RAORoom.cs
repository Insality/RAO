using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RAOServer.Network;
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
        public int MaxPlayers = 4;
        private readonly List<Player.Player> _players;
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

        public int GetPlayersCount() {
            return _players.Count;
        }

        public string GetStringMap() {
            var str = "";
            foreach (var tileRow in _map.Tiles) {
                str += string.Join("", tileRow);
                str += '\n';
            }
            return str;
        }

        public JObject GetInfo() {
            var rm = new JObject();
            rm.Add("Id", Id);
            rm.Add("Players", GetPlayersCount());
            rm.Add("MaxPlayers", MaxPlayers);
            return rm;
        }

        internal void ConnectPlayer(RAOConnection connection) {
            if (_players.Count >= MaxPlayers){
                throw new GameRoomMaxPlayers();
            }

            _players.Add(connection.Player);
            connection.Player.CurrentRoom = Id;
            // DEBUG, TEST
            connection.SendData(GetStringMap());
        }
    }
}