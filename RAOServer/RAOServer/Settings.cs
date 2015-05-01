namespace RAOServer {
    internal class Settings {
        // TODO: Сделать загрузку из ini файла при старте

        /** General settings */
        public const string GameName = "Roguelike Arena Online";
        public const string GameRoute = "/rao";


        /** Logs && Statistics settings */
        public const bool IsDebug = true;
        public const string LogFileName = "ROA_Server.log";
        public const bool IsLogToFile = false;

        /** Network settings */
        public const int Port = 4080;
        public const int MaxConnections = 16;
    }
}