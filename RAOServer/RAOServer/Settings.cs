﻿using IniParser;
using IniParser.Model;

namespace RAOServer {
    internal class Settings {
        // TODO: Сделать загрузку из ini файла при старте

        private static readonly FileIniDataParser Parser = new FileIniDataParser();
        private static readonly IniData Data = Parser.ReadFile("settings.ini");

        /** General settings */
        public static string GameName = Data["Server"]["GameName"];
        public static string GameRoute = Data["Server"]["GameRoute"];
        public static string Python3Path = Data["Server"]["Python3Path"];
        public static string NameGenScript = Data["Server"]["NameGenScript"];


        /** Logs && Statistics settings */
        public static bool IsDebug = bool.Parse(Data["Log"]["IsDebug"]);
        public static string LogFileName = Data["Log"]["LogFileName"];
        public static bool IsLogToFile = bool.Parse(Data["Log"]["IsLogToFile"]);

        /** Network settings */
        public static string Ip = Data["Network"]["Ip"];
        public static int Port = int.Parse(Data["Network"]["Port"]);
    }
}