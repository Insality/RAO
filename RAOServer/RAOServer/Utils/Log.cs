using System;

namespace RAOServer.Utils {
    internal class Log {
        public static void Info(string text) {
            Console.ForegroundColor = ConsoleColor.White;
            _print(text);
        }

        public static void Debug(string text) {
            if (Settings.IsDebug){
                Console.ForegroundColor = ConsoleColor.Yellow;
                _print(text);
            }
        }

        public static void Network(string text) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            _print(text);
        }

        public static void Error(string text) {
            Console.ForegroundColor = ConsoleColor.Red;
            _print(text);
        }

        public static void Game(string text) {
            Console.ForegroundColor = ConsoleColor.Green;
            _print(text);
        }

        public static void Terminal(string text) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }

        private static void _print(string text) {
            string curTime = DateTime.Now.ToString("[HH:mm:ss]: ");
            Console.WriteLine(curTime + text);
            Console.ForegroundColor = ConsoleColor.White;

            if (Settings.IsLogToFile)
                _printToLogFile(text);
        }


        private static void _printToLogFile(string text) {
            throw new NotImplementedException();
        }
    }
}
