using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RAOServer.Network;
using RAOServer.Utils;

namespace RAOServer.Game {
    /// <summary>
    ///     RAORoom - игровая комната одной арены. Создается при создании лобби
    ///     и обрабатывает все действия в этой комнате
    /// </summary>
    internal class RAORoom {
        private static int _roomCounter;
        private readonly Map _map = new Map();
        private readonly List<Player.Player> _players;
        public int Id;
        public int MaxPlayers = 4;
        public string State;
        private RAOServer _server;

        public RAORoom(RAOServer server) {
            Id = _roomCounter++;
            State = States.RoomWaiting;
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
            foreach (var tileRow in _map.Tiles){
                str += string.Join("", tileRow);
                str += '\n';
            }
            return str;
        }

        public JObject GetInfo() {
            var info = new JObject {
                {"Id", Id},
                {"Players", GetPlayersCount()},
                {"MaxPlayers", MaxPlayers},
                {"State", State}
            };
            return info;
        }

        public JToken GetPlayersInfo() {
            var playersInfo = _players.Select(room => room.GetInfo()).ToList();
            return JToken.FromObject(playersInfo);
        }

        internal void ConnectPlayer(Player.Player player) {
            if (_players.Count >= MaxPlayers){
                throw new GameRoomMaxPlayers();
            }

            _players.Add(player);
            player.ConnectToRoom(Id);

            Log.Game(string.Format("Player {0} joined to room {1}", player.Name, Id));
        }

        internal void DisconnectPlayer(Player.Player player) {
            _players.Remove(player);
            player.ConnectToLobby();

            Log.Game(string.Format("Player {0} leave room {1}", player.Name, Id));
        }
    }
}