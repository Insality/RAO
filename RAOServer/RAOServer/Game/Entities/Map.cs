using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using RAOServer.Game.Entities.Enviroment;
using RAOServer.Game.PlayerStuff;
using RAOServer.Utils;

namespace RAOServer.Game {
    internal class Map {
        private readonly RAORoom _room;
        public string Name;
        public List<Tile> Tiles;
        private int mapWidth;
        private int mapHeight;

        public Map(RAORoom room) {
            Name = NameGen.GenLocation();
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

            mapWidth = tileMap[0].Length;
            mapHeight = tileMap.Count;
            for (var i = 0; i < tileMap.Count; i++){
                for (var j = 0; j < tileMap[i].Length; j++){
                    switch (tileMap[i][j]){
                        case '_':
                            entities.Add(new PressurePlate(j, i, _room));
                            break;
                        case 'D':
                            entities.Add(new Door(j, i, _room));
                            break;
                        case 'T':
                            entities.Add(new Trap(j, i, _room));
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
                    listWall.Add(new List<int> {int.Parse(tile["X"].ToString()), int.Parse(tile["Y"].ToString())});
                if (tile["Sym"].ToString() == ".")
                    listFloor.Add(new List<int> {int.Parse(tile["X"].ToString()), int.Parse(tile["Y"].ToString())});
            }

            var info = new JObject {
                {"#", JToken.FromObject(listWall)},
                {".", JToken.FromObject(listFloor)},
            };
            return info;
        }

        public Tile GetTile(int x, int y) {
            return Tiles.Find(t=>t.X == x && t.Y == y);
        }


        /// <summary>
        ///     Lightcast function, Field of view
        /// </summary>
        public List<Tile> GetFov(Hero e) {
            var radius = e.SeeRadius.Current;
            // Multipliers for transforming coordinates to other octants:
            int[,] mult = {
                {1, 0, 0, -1, -1, 0, 0, 1},
                {0, 1, -1, 0, 0, -1, 1, 0},
                {0, 1, 1, 0, 0, -1, -1, 0},
                {1, 0, 0, 1, -1, 0, 0, -1}
            };

            var tiles = new List<Tile>();
            for (var oct = 0; oct < 8; oct++){
                _castLight(e.X, e.Y, 1, 1.0, 0.0, radius, mult[0, oct], mult[1, oct], mult[2, oct], mult[3, oct], 0,
                    tiles);
            }

            tiles.Add(GetTile(e.X, e.Y));

            return tiles;
        }

        /// <summary>
        ///     recursive lightcast function
        /// </summary>
        private void _castLight(int cx, int cy, int row, double start, double end, int radius, int xx, int xy, int yx,
            int yy,
            int id, List<Tile> tiles) {
            if (start < end){
                return;
            }
            var newStart = start;

            var radiusSquared = radius*radius;
            for (var j = row; j <= radius; j++){
                var dx = -j - 1;
                var dy = -j;
                bool blocked = false;
                while (dx <= 0){
                    dx += 1;
                    // Translate the dx and dy coord, into map coord
                    var x = cx + dx*xx + dy*xy;
                    var y = cy + dx*yx + dy*yy;
                    // l_slope and r_slope store the slopes of the left and right
                    // extremities of the square we're considering:
                    var lSlope = (dx - 0.5)/(dy + 0.5);
                    var rSlope = (dx + 0.5)/(dy - 0.5);
                    if (start < rSlope){
                        continue;
                    } else if (end > lSlope){
                        break;
                    }
                    else{
                        // Our light beam is touching this square; light it:
                        if (dx * dx + dy * dy < radiusSquared && x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
                        {
                            tiles.Add(GetTile(x, y));
                        }
                        if (blocked){
                            //  we're scanning a row of blocked squares:
                            if (_blocked(x, y)){
                                newStart = rSlope;
//                                continue;
                            }
                            else{
                                blocked = false;
                                start = newStart;
                            }
                        }
                        else{
                            if (_blocked(x, y) && j < radius){
                                blocked = true;
                                _castLight(cx, cy, j+1, start, lSlope, radius, xx, xy, yx, yy, id+1, tiles);
                                newStart = rSlope;
                            }
                        }
                    }
                }
                if (blocked){
                    break;
                }
            }
        }

        private bool _blocked(int x, int y) {
            return (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight || GetTile(x, y).IsSolid);
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