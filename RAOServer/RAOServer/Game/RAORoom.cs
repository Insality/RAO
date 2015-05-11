using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Entities.Enviroment;
using RAOServer.Game.PlayerStuff;
using RAOServer.Network;
using RAOServer.Utils;

namespace RAOServer.Game {
    /// <summary>
    ///     RAORoom - игровая комната одной арены. Создается при создании лобби
    ///     и обрабатывает все действия в этой комнате
    /// </summary>
    internal class RAORoom {
        private static int _roomCounter;
        private readonly Map _map;
        private readonly List<Player> _players;
        private readonly Timer timer;
        public List<Entity> Entities;
        public int Id;
        public int MaxPlayers;
        public RoomStates State;
        public int Turn;

        public RAORoom(int maxPlayers, int turnTime) {
            _map = new Map(this);
            Id = _roomCounter++;
            Turn = 0;
            State = RoomStates.RoomWaiting;
            MaxPlayers = maxPlayers;
            _players = new List<Player>();
            

            Entities = new List<Entity>();

            timer = new Timer(turnTime);
            timer.Elapsed += OnTimedEvent;
            timer.Start();
            GC.KeepAlive(timer);

            _map.LoadMapFromFile("testMap.txt");
        }

        public List<Tile> GetTiles() {
            return _map.Tiles;
        }

        public List<Entity> GetEntities(int x, int y) {
            return Entities.Where(ent=>ent.X == x && ent.Y == y).ToList();
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            GameTick();
        }

        public JToken GetMapInfo(Player pl) {
            List<JObject> mapInfo;
            if (pl == null){
                mapInfo = _map.Tiles.Select(tile=>tile.GetInfo()).ToList();
            }
            else{
                var h = pl.Hero;
                mapInfo = _map.Tiles.Where(t=> h.DistanceTo(t.X, t.Y) <= h.SeeRadius.Current)
                    .Select(tile => tile.GetInfo()).ToList();
            }
            return Map.CompressMapList(mapInfo);
        }

        public JObject GetInfo() {
            var info = new JObject {
                {"Id", Id},
                {"Players", _players.Count},
                {"MaxPlayers", MaxPlayers},
                {"State", State.ToString()},
                {"Map", _map.GetInfo()},
                {"TurnTime", timer.Interval}
            };
            return info;
        }

        public void GameTick() {
            // Do all player actions:
            var players = _players.OrderBy(pl=>pl.Hero.Initiative.Current).ToList();
            foreach (var player in players){
                player.Hero.Action();
            }

            // Send to all players game step info:


            foreach (var pl in _players){
                var sm = new ServerMessage { Code = 200, Type = MsgDict.ServerInformation };

                var data = new JObject();
                data.Add("map", GetMapInfo(pl));
                data.Add("players", GetPlayersInfo());
                data.Add("entities", GetEntitiesInfo(pl));
                data.Add("tick", true);
                sm.Data = data.ToString(Formatting.None).Replace('"', '\'');

                pl.Connection.SendData(sm.Serialize());
            }

            Turn++;
        }

        public JToken GetPlayersInfo() {
            var playersInfo = _players.Select(pl=>pl.GetInfo()).ToList();
            return JToken.FromObject(playersInfo);
        }

        public JToken GetEntitiesInfo(Player pl) {
            List<JObject> entitiesInfo;
            if (pl == null){
                entitiesInfo = Entities.Select(entitity=>entitity.GetInfo()).ToList();
            }
            else{
                var h = pl.Hero;
                entitiesInfo = Entities.Where(e => (Math.Sqrt(Math.Pow(h.X - e.X, 2) + Math.Pow(h.Y - e.Y, 2))) <= h.SeeRadius.Current)
                    .Select(e => e.GetInfo()).ToList();
            }
            return JToken.FromObject(entitiesInfo);
        }


        internal void ConnectPlayer(Player player) {
            if (_players.Count >= MaxPlayers){
                throw new GameRoomMaxPlayers();
            }

            _players.Add(player);
            player.ConnectToRoom(Id);

            Log.Game(string.Format("Player {0} joined to room {1}", player.Name, Id));
            ChatToRoom(string.Format("Player {0} joined to room", player.Name), ":");

            player.Hero = new Hero(3, 5, this);
            Entities.Add(player.Hero);
        }

        internal void DisconnectPlayer(Player player) {
            Entities.Remove(player.Hero);
            _players.Remove(player);
            player.ConnectToLobby();

            Log.Game(string.Format("Player {0} leave room {1}", player.Name, Id));
            ChatToRoom(string.Format("Player {0} leave room", player.Name), ":");
        }

        internal void ChatToRoom(string msg, string sender) {
            var location = "Room#" + Id;

            msg = string.Format("[{0}] {1}: {2}", location, sender, msg);
            Log.Game(msg);

            var sm = new ServerMessage {Code = MsgDict.CodeSuccessful, Type = MsgDict.ServerInformation};
            var data = new JObject {{"chat", msg}};
            sm.Data = data.ToString(Formatting.None).Replace('"', '\'');

            foreach (var pl in _players){
                pl.Connection.SendData(sm.Serialize());
            }
        }
    }
}