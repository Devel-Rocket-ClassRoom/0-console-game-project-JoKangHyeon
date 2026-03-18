using System;
using System.Collections.Generic;
using System.Text;

namespace TruckGame.TruckGame
{
    public class SaveData
    {
        public string PlayerName;
        public List<Character> Characters;
        public List<string> DeadNames;
        public int Scrap;
        public int Food;
        public int Days;

        public List<int> Rounds;
    }

    public class Character
    {
        public enum CharacterType
        {

        }

        public string Name;
        public int Level;

        public int MaxHealth;
        public int Health;

        public Weapon Weapon;
        public Armor Armor;

        public CharacterType type;
    }
}
