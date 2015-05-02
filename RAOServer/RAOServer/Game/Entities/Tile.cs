namespace RAOServer.Game {
    internal class Tile {
        public bool IsSolid;
        public char Symbol;

        public Tile(char Symbol) {
            // TODO: Загрушка, исправить на загрузку из словаря
            if (Symbol == '#'){
                IsSolid = true;
            }
            else{
                IsSolid = false;
            }
            this.Symbol = Symbol;
        }

        public void Update() {
            
        }

        public override string ToString() {
            return Symbol.ToString();
        }
    }
}