using IniParser;
using IniParser.Model;

namespace RAOServer {
    internal class Settings {
        // TODO: Сделать загрузку из ini файла при старте

        static FileIniDataParser parser = new FileIniDataParser();
        static IniData data = parser.ReadFile("settings.ini");

        /** General settings */
        public static string GameName = data["Server"]["GameName"];
        public static string GameRoute = data["Server"]["GameRoute"];


        /** Logs && Statistics settings */
        public static bool IsDebug = bool.Parse(data["Log"]["IsDebug"]);
        public static string LogFileName = data["Log"]["LogFileName"];
        public static bool IsLogToFile = bool.Parse(data["Log"]["IsLogToFile"]);

        /** Network settings */
        public static string Ip = data["Network"]["Ip"];
        public static int Port = int.Parse(data["Network"]["Port"]);

    }
}