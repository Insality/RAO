using System;
using System.Collections.Generic;
using System.Linq;
using RAOServer.Game;
using RAOServer.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RAOServer {
    internal class ServerConsole {
        private readonly List<string> _commandsList = new List<string> {
            "help",
            "list",
            "roomlist",
            "newroom",
            "removeroom",
            "exit"
        };

        private RAOServer server;
        private WebSocketServer ws;

        public ServerConsole(RAOServer server) {
            this.server = server;
            ws = server.GetSocketServer();
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

                    foreach (var player in server.GetPlayers()){
                        Log.Terminal("Player: " + player);
                    }
                    break;
                case "roomlist":
                    foreach (var room in server.GetRooms()){
                        Log.Terminal("Room: " + room.Id);
                    }
                    break;
                case "newroom":
                    var newRoom = new RAORoom(server);
                    Log.Network("New room: " + newRoom.Id);
                    server.GetRooms().Add(newRoom);
                    break;
                case "removeroom":
                    server.GetRooms().Remove(server.GetRooms().Find(room=>room.Id == int.Parse(arg)));
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