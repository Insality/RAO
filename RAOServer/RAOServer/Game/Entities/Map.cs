using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Entities.Enviroment;
using RAOServer.Utils;

namespace RAOServer.Game {
    internal class Map {
        public string Name;
        public List<List<Tile>> Tiles;
        private RAORoom _room;

        public Map(RAORoom room) {
            Name = "Unnamed dungeon";
            _room = room;
        }

        public void LoadMapFromFile(string filename) {
            Tiles = new List<List<Tile>>();
            var entities = _room.Entities;

            var s = File.ReadAllText(filename);
            var tileMap = new List<string>(s.Split('\n'));

            for (var i = 0; i < tileMap.Count; i++){
                tileMap[i] = tileMap[i].TrimEnd('\r');
            }

            for (var i = 0; i < tileMap.Count; i++){
                var tilesRow = new List<Tile>();
                for (var j = 0; j < tileMap[i].Length; j++){
                    Tile tile;
                    switch (tileMap[i][j]){
                        case '#':
                            tile = new Tile(tileMap[i][j]);
                            break;
                        case '.':
                            tile = new Tile(tileMap[i][j]);
                            break;
                        case '_':
                            tile = new Tile(tileMap[i][j]);
                            entities.Add(new PressurePlate(j, i, _room));
                            break;
                        default:
                            tile = new Tile(tileMap[i][j]);
                            Log.Error("Error tile ASCII code in LoadMapFromFile()");
                            break;
                    }
                    tilesRow.Add(tile);
                }
                Tiles.Add(tilesRow);
            }
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