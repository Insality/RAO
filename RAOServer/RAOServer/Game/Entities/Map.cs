using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Entities.Enviroment;
using RAOServer.Utils;

namespace RAOServer.Game {
    internal class Map {
        public string Name;
        public List<Tile> Tiles;
        private readonly RAORoom _room;

        public Map(RAORoom room) {
            Name = "Unnamed dungeon";
            _room = room;
        }

        public void LoadMapFromFile(string filename) {
            Tiles = new List<Tile>();
            var entities = _room.Entities;

            var s = File.ReadAllText(filename);
            var tileMap = new List<string>(s.Split('\n'));

            for (var i = 0; i < tileMap.Count; i++){
                tileMap[i] = tileMap[i].TrimEnd('\r');
            }

            for (var i = 0; i < tileMap.Count; i++){
                for (var j = 0; j < tileMap[i].Length; j++){
                    Tile tile;
                    switch (tileMap[i][j]){
                        case '_':
                            entities.Add(new PressurePlate(j, i, _room));
                            break;
                    }
                    Tiles.Add(new Tile(j, i, tileMap[i][j]));
                }
            }
        }

        public static JObject CompressMapList(List<JObject> tiles) {
            var listWall = new List<List<int>>();
            var listFloor = new List<List<int>>();

            foreach (var tile in tiles){
                if (tile["Sym"].ToString() == "#")
                    listWall.Add(new List<int>{int.Parse(tile["X"].ToString()), int.Parse(tile["Y"].ToString())} );
                if (tile["Sym"].ToString() == ".")
                    listFloor.Add(new List<int>{int.Parse(tile["X"].ToString()), int.Parse(tile["Y"].ToString())} );
            }

            var info = new JObject {
                {"#", JToken.FromObject(listWall)},
                {".", JToken.FromObject(listFloor)},
            };
            return info;
        } 


        public JObject GetInfo() {
            var info = new JObject {
                {"Name", Name},
                {"Size", Tiles.Count},
            };
            return info;
        }
    }
}