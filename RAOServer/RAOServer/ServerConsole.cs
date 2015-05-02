using System;
using System.Collections.Generic;
using System.Linq;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class ServerConsole {
        private readonly List<string> _commandsList = new List<string> {
            "help",
            "list",
            "exit"
        };

        private WebSocketServer ws;

        public ServerConsole(WebSocketServer ws) {
            this.ws = ws;
        }

        public void Start() {
            while (true){
                Console.Write(">>> ");
                ProcessCommand(Console.ReadLine());
            }
        }

        private void ProcessCommand(string command) {
            string arg = command.Split(' ').Last();
            command = command.Split(' ').First();
            switch (command){
                case "list":
                    Log.Terminal("Connections: " + ws.WebSocketServices.SessionCount);
                    Log.Terminal("IDs: " + String.Join(", ", ws.WebSocketServices[Settings.GameRoute].Sessions.IDs));
                    break;
                case "help":
                    Log.Terminal(String.Join("\n", _commandsList));
                    break;
                case "exit":
                    ws.Stop(CloseStatusCode.Normal, "Shutdown");
                    Environment.Exit(0);
                    break;
                default:
                    Log.Terminal("Unknown command, print help to command list");
                    break;
            }
        }
    }
}