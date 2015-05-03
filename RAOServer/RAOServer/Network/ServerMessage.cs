using Newtonsoft.Json;

namespace RAOServer.Network {
    internal class ServerMessage {
        [JsonProperty(PropertyName = "code")] public int Code;
        [JsonProperty(PropertyName = "data")] public string Data;
        [JsonProperty(PropertyName = "error")] public string Error;
        [JsonProperty(PropertyName = "type")] public string Type;


        public static string ResponseCode(int errorCode) {
            var serverMessage = new ServerMessage {
                Code = errorCode,
                Data = "{}",
                Error = MsgDict.MsgCode[errorCode],
                Type = MsgDict.ServerResponse
            };

            return serverMessage.Serialize();
        }

        public string Serialize() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}