using System;
using System.Collections.Generic;
using System.Linq;
using RAOServer.Utils;

namespace RAOServer {
    internal class ServerConsole {
        private readonly List<string> _commandsList = new List<string> {
            "help",
            "exit"
        };

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
                case "help":
                    Log.Terminal(String.Join("\n", _commandsList));
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Log.Terminal("Unknown command, print help to command list");
                    break;
            }
        }
    }
}