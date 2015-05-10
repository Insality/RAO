

using System;
using WebSocketSharp;

namespace RAOClientSharp {
    class Program {
        static void Main(string[] args) {

            using (var ws = new WebSocket("ws://localhost:4080/rao")) {
                ws.OnMessage += (sender, e) =>
                  Console.WriteLine("Response says: " + e.Data);
                ws.OnOpen += (sender, e)=>Console.WriteLine("OPEN");

                ws.Connect();
                ws.Send("{\"type\":\"connect\",\"api\":\"1.0\",\"data\":{\"login\":\"\",\"password\":\"\"}}");
                Console.WriteLine("Sended");

                var com = "";
                while (com != "exit"){
                    com = Console.ReadLine();
                    ws.Send(com);
                }
            }
        }
    }
}
