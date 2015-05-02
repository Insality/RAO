using RAOServer.Utils;

namespace RAOServer {
    internal class Run {
        private static void Main(string[] args) {
            Log.Info("Starting the RAO Server...");
            var server = RAOServer.Instance;
            server.Start();
        }
    }
}