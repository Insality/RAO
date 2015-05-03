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
            Code = MsgDict.AuthNotLoggedIn;
        }
    }

    public class AlreadyLogged: RAOException {
        public AlreadyLogged() {
            Code = MsgDict.CodeAlreadyLogged;
        }
    }

    public class PlayerNotInGame: RAOException {
        public PlayerNotInGame() {
            Code = MsgDict.PlayerNotInGame;
        }
    }

    public class PlayerNotInLobby: RAOException {
        public PlayerNotInLobby() {
            Code = MsgDict.PlayerNotInLobby;
        }
    }

    public class GameRoomMaxPlayers: RAOException {
        public GameRoomMaxPlayers() {
            Code = MsgDict.GameRoomMaxPlayers;
        }
    }
}