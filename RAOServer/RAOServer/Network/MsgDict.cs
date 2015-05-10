using System.Collections.Generic;

namespace RAOServer.Network {
    internal class MsgDict {
        public const string ClientConnect = "connect";
        public const string ClientDisconnect = "disconnect";
        public const string ClientConnectRoom = "connect_room";
        public const string ClientControl = "control";
        public const string ClientRequest = "request";
        public const string ClientStatus = "status";
        public const string ClientChat = "chat";


        public const string ServerInformation = "information";
        public const string ServerStatus = "status";
        public const string ServerResponse = "response";


        public const int CodeSuccessful = 200;
        public const int CodeForbidden = 403;
        public const int CodeServerError = 500;
        public const int CodeIncorrectApiVersion = 1;
        public const int CodeIncorrectDataFormat = 2;
        public const int CodeIncorrectDataValues = 3;
        public const int CodeIncorrectLoginOrPassword = 4;
        public const int CodeAlreadyLogged = 5;
        public const int CodeNotLoggedIn = 6;
        public const int CodePlayerNotInGame = 10;
        public const int CodePlayerNotInLobby = 11;
        public const int CodeRoomMaxPlayers = 20;


        public static readonly List<string> ClientRequestKeys = new List<string> {"type", "api", "data"};
        public static readonly List<string> ClientConnectKeys = new List<string> {"login", "password"};

        public static readonly List<string> ClientControlCommands = new List<string> {
            "control_up",
            "control_down",
            "control_left",
            "control_right",
            "control_action"
        };

        public static readonly List<string> ClientTypeValues = new List<string> {
            ClientConnect,
            ClientConnectRoom,
            ClientControl,
            ClientDisconnect,
            ClientRequest,
            ClientStatus,
            ClientChat
        };


        public static readonly List<string> AviableApi = new List<string> {"1.0"};


        public static Dictionary<int, string> MsgCode = new Dictionary<int, string> {
            {CodeSuccessful, "successful"},
            {CodeForbidden, "forbidden"},
            {CodeServerError, "server.error"},
            {CodeIncorrectApiVersion, "api.incorrect_version"},
            {CodeIncorrectDataFormat, "json.incorrect_format"},
            {CodeIncorrectDataValues, "json.incorrect_data"},
            {CodeIncorrectLoginOrPassword, "auth.incorrect_login_or_password"},
            {CodeNotLoggedIn, "auth.not_logged_in"},
            {CodeAlreadyLogged, "auth.already_logged"},
            {CodePlayerNotInGame, "player.not_in_game"},
            {CodePlayerNotInLobby, "player.not_in_lobby"},
            {CodeRoomMaxPlayers, "game.room.max_players"}
        };
    }
}