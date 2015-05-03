using System.Collections.Generic;

namespace RAOServer.Network {
    internal class MsgDict {
        public const string ClientConnect = "connect";
        public const string ClientDisconnect = "disconnect";
        public const string ClientConnectRoom = "connect_room";
        public const string ClientControl = "control";
        public const string ClientRequest = "request";
        public const string ClientStatus = "status";


        public const string ServerInformation = "information";
        public const string ServerStatus = "status";
        public const string ServerResponse = "response";


        public const int CodeSuccessful = 200;
        public const int CodeForbidden = 403;
        public const int CodeIncorrectApiVersion = 1;
        public const int CodeIncorrectDataFormat = 2;
        public const int CodeIncorrectDataValues = 3;
        public const int CodeIncorrectLoginOrPassword = 4;
        public const int CodeAlreadyLogged = 5;


        public static readonly List<string> ClientRequestKeys = new List<string> {"type", "api", "data"};
        public static readonly List<string> ClientConnectKeys = new List<string> {"login", "password"};

        public static readonly List<string> ClientTypeValues = new List<string> {
            ClientConnect,
            ClientConnectRoom,
            ClientControl,
            ClientDisconnect,
            ClientRequest,
            ClientStatus
        };


        public static readonly List<string> AviableApi = new List<string> {"1.0"};


        public static Dictionary<int, string> MsgCode = new Dictionary<int, string> {
            {CodeSuccessful, "successful"},
            {CodeForbidden, "forbidden"},
            {CodeIncorrectApiVersion, "api.incorrect_version"},
            {CodeIncorrectDataFormat, "json.incorrect_format"},
            {CodeIncorrectDataValues, "json.incorrect_data"},
            {CodeIncorrectLoginOrPassword, "auth.incorrect_login_or_password"},
            {CodeAlreadyLogged, "auth.already_logged"}
        };
    }
}