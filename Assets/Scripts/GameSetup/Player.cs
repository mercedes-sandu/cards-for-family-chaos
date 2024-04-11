using System;
using CCSS;
using UnityEngine;
using Utility;

namespace GameSetup
{
    public class Player : MonoBehaviour
    {
        public const string PlayerRole = "[[P]]";
        public static Character PlayerCharacter;
        public static bool PlayerCharacterFamilyOne; // todo: why did i need this?

        private static int _reputation;
        private static int _money;
        private static int _health;

        public enum Stat
        {
            Reputation,
            Money,
            Health
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            GameEvent.OnChoiceMade += UpdateStats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        private void UpdateStats(Choice choice)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static int GetPlayerStat(Stat stat) => stat switch
        {
            Stat.Reputation => _reputation,
            Stat.Money => _money,
            Stat.Health => _health,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, "Tried to get invalid player stat.")
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetPlayerStat(Stat stat, int value)
        {
            switch (stat)
            {
                case Stat.Reputation:
                    _reputation = value;
                    break;
                case Stat.Money:
                    _money = value;
                    break;
                case Stat.Health:
                    _health = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, "Tried to set invalid player stat.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnChoiceMade -= UpdateStats;
        }
    }
}