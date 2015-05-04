namespace RAOServer.Game {
    internal class Tile {
        public bool IsSolid;
        public char Symbol;

        public Tile(char symbol) {
            // TODO: Загрушка, исправить на загрузку из словаря
            if (symbol == '#'){
                IsSolid = true;
            }
            else{
                IsSolid = false;
            }
            Symbol = symbol;
        }

        public void Update() {
        }

        public override string ToString() {
            return Symbol.ToString();
        }
    }
}