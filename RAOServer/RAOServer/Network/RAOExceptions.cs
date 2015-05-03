using System;

namespace RAOServer.Network {
    public class RAOException: Exception {
        public int Code;
    }

    public class InvalidDataFormat: RAOException {
        public InvalidDataFormat() {
            Code = MsgDict.CodeIncorrectDataFormat;
        }
    }

    public class InvalidDataValues: RAOException {
        public InvalidDataValues() {
            Code = MsgDict.CodeIncorrectDataValues;
        }
    }

    public class InvalidApiVersion: RAOException {
        public InvalidApiVersion() {
            Code = MsgDict.CodeIncorrectApiVersion;
        }
    }

    public class InvalidLoginOrPassword: RAOException {
        public InvalidLoginOrPassword() {
            Code = MsgDict.CodeIncorrectLoginOrPassword;
        }
    }

    public class NotLoggedIn: RAOException {
        public NotLoggedIn() {
            Code = MsgDict.CodeNotLoggedIn;
        }
    }

    public class AlreadyLogged: RAOException {
        public AlreadyLogged() {
            Code = MsgDict.CodeAlreadyLogged;
        }
    }

    public class PlayerNotInGame: RAOException {
        public PlayerNotInGame() {
            Code = MsgDict.CodePlayerNotInGame;
        }
    }

    public class PlayerNotInLobby: RAOException {
        public PlayerNotInLobby() {
            Code = MsgDict.CodePlayerNotInLobby;
        }
    }

    public class GameRoomMaxPlayers: RAOException {
        public GameRoomMaxPlayers() {
            Code = MsgDict.CodeRoomMaxPlayers;
        }
    }
}