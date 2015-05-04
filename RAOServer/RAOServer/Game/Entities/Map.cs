using System.Collections.Generic;
using System.IO;
using RAOServer.Utils;

namespace RAOServer.Game {
    internal class Map {
        public List<List<Tile>> Tiles;

        public void LoadMapFromFile(string filename) {
            Tiles = new List<List<Tile>>();

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
    }
}