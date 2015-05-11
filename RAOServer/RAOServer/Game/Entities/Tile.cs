using Newtonsoft.Json.Linq;

namespace RAOServer.Game {
    internal class Tile {
        public bool IsSolid;
        public char Symbol;
        public int X;
        public int Y;

        public Tile(int x, int y, char symbol) {
            // TODO: Загрушка, исправить на загрузку из словаря
            if (symbol == '#'){
                IsSolid = true;
            }
            else{
                IsSolid = false;
            }
            Symbol = symbol;
            X = x;
            Y = y;
        }

        public void Update() {
        }

        public override string ToString() {
            return Symbol.ToString();
        }

        public virtual JObject GetInfo() {
            var info = new JObject {
                {"Sym", Symbol.ToString()},
                {"X", X},
                {"Y", Y}
            };
            return info;
        }
    }
}