﻿using System;
using RAOServer.Utils;

namespace RAOServer.Game {
    /// <summary>
    ///     RAORoom - игровая комната одной арены. Создается при создании лобби
    ///     и обрабатывает все действия в этой комнате
    /// </summary>
    internal class RAORoom {
        private readonly Map map = new Map();

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