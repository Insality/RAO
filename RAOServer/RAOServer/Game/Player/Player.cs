﻿using Newtonsoft.Json.Linq;
using RAOServer.Network;

namespace RAOServer.Game.Player {
    internal class Player {
        public int CurrentRoom;
        public Hero Hero;
        public string Id;
        public int Level;
        public string Name;
        public string State;
        public PlayerStatistic Statistic;


        public Player(string id, string name) {
            Id = id;
            Name = name;
            CurrentRoom = -1;
            State = States.PlayerLobby;
            Hero = new Hero();
        }

        public override string ToString() {
            return Name + ":" + Id;
        }

        public JToken GetInfo() {
            var info = new JObject {{"Name", Name}, {"Hero", Hero.GetInfo()}, {"CurrentRoom", CurrentRoom}, {"State", State}};
            return info;
        }

        public void ConnectToRoom(int id) {
            if (State != States.PlayerLobby){
                throw new PlayerNotInLobby();
            }

            CurrentRoom = id;
            State = States.PlayerGame;
        }

        public void ConnectToLobby() {
            if (State != States.PlayerGame) {
                throw new PlayerNotInGame();
            }

            CurrentRoom = -1;
            State = States.PlayerLobby;
        }
    }
}