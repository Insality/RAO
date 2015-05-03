using System;

namespace RAOServer.Network {
    public class InvalidDataFormat: Exception {
    }

    public class InvalidDataValues : Exception {
    }

    public class InvalidApiVersion : Exception {
    }

    public class InvalidLoginOrPassword: Exception {
    }

    public class AlreadyLogged: Exception {
    }
}