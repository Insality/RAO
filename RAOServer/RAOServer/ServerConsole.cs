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
            "listroom",
            "newroom",
            "removeroom",
            "exit"
        };

        private readonly RAOServer _server;
        private readonly WebSocketServer _ws;

        public ServerConsole(RAOServer server) {
            _server = server;
            _ws = server.GetSocketServer();
        }

        public void Start() {
            while (true){
                Console.Write(">>> ");
                ProcessCommand(Console.ReadLine());
            }
        }

        private void ProcessCommand(string command) {
            var arg = command.Split(' ').Last();
            command = command.Split(' ').First();
            switch (command){
                case "list":
                    _commandList();
                    return;
                case "listroom":
                    _commandListRoom();
                    return;
                case "newroom":
                    _commandNewRoom();
                    return;
                case "removeroom":
                    _commandRemoveRoom(arg);
                    return;
                case "help":
                    _commandHelp();
                    return;
                case "status":
                    _commandStatus();
                    return;
                case "exit":
                    _commandExit();
                    return;
                default:
                    Log.Terminal("Unknown command, print help to command list");
                    break;
            }
        }

        private void _commandList() {
            Log.Terminal("Connections: " + _ws.WebSocketServices.SessionCount);

            foreach (var player in _server.GetPlayers()){
                Log.Terminal("Player: " + player);
            }
        }

        private void _commandListRoom() {
            foreach (var room in _server.GetRooms()){
                Log.Terminal("Room: " + room.Id);
            }
        }

        private void _commandNewRoom() {
            var newRoom = new RAORoom(_server);
            Log.Network("New room: " + newRoom.Id);
            _server.GetRooms().Add(newRoom);
        }

        private void _commandRemoveRoom(string arg) {
            _server.GetRooms().Remove(_server.GetRooms().Find(room=>room.Id == int.Parse(arg)));
        }

        private void _commandHelp() {
            Log.Terminal(String.Join("\n", _commandsList));
        }

        private void _commandStatus() {
            Log.Terminal(_ws.KeepClean.ToString());
            Log.Terminal(_ws.WebSocketServices[Settings.GameRoute].Type.ToString());
            Log.Terminal(string.Join(", ", _ws.WebSocketServices[Settings.GameRoute].Sessions.ActiveIDs));
            Log.Terminal("INACTIVES");
            Log.Terminal(string.Join(", ", _ws.WebSocketServices[Settings.GameRoute].Sessions.InactiveIDs));
            Log.Terminal(string.Join(", ", _ws.WebSocketServices[Settings.GameRoute].Sessions.Sessions));

            foreach (RAOConnection ses in _ws.WebSocketServices[Settings.GameRoute].Sessions.Sessions){
                Log.Terminal(string.Format("{0}: {1}, {2}", ses.Player.Name, ses.ID, ses.State));
            }
            
        }

        private void _commandExit() {
            _ws.Stop(CloseStatusCode.Normal, "Shutdown");
            Environment.Exit(0);
        }
    }
}