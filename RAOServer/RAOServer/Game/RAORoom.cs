using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Player;
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

        private Timer timer;

        public RAORoom(RAOServer server) {
            Id = _roomCounter++;
            State = States.RoomWaiting;
            _players = new List<Player.Player>();
            _server = server;
            _map.LoadMapFromFile("testMap.txt");

            timer = new Timer(250);
            timer.Elapsed += OnTimedEvent;
            timer.Start();
            GC.KeepAlive(timer);
        }

        public List<List<Tile>> GetTiles() {
            return _map.Tiles;
        }

        public int GetPlayersCount() {
            return _players.Count;
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            GameTick();
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
                {"State", State},
                {"Map", _map.GetInfo()},
                {"TurnTime", timer.Interval}
            };
            return info;
        }

        public void GameTick() {
            // Send to all players game step info:
            var sm = new ServerMessage { Code = 200, Type = MsgDict.ServerInformation };

            var data = new JObject();
            data.Add("map", GetStringMap());
            data.Add("players", GetPlayersInfo());
            data.Add("tick", true);
            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');

            foreach (var pl in _players){
                pl.Connection.SendData(sm.Serialize());
            }
        }

        public JToken GetPlayersInfo() {
            var playersInfo = _players.Select(room=>room.GetInfo()).ToList();
            return JToken.FromObject(playersInfo);
        }

        internal void ConnectPlayer(Player.Player player) {
            if (_players.Count >= MaxPlayers){
                throw new GameRoomMaxPlayers();
            }

            _players.Add(player);
            player.ConnectToRoom(Id);

            Log.Game(string.Format("Player {0} joined to room {1}", player.Name, Id));

            player.Hero = new Hero(this) {X = 3, Y = 5};
        }

        internal void DisconnectPlayer(Player.Player player) {
            _players.Remove(player);
            player.ConnectToLobby();

            Log.Game(string.Format("Player {0} leave room {1}", player.Name, Id));
        }
    }
}