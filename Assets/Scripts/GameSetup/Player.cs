using System;
using UnityEngine;

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
        /// Returns the current value of the specified player's stat.
        /// </summary>
        /// <param name="stat">The stat to get the value of.</param>
        /// <returns>The current stat value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the specified stat is invalid.</exception>
        public static int GetPlayerStat(Stat stat) => stat switch
        {
            Stat.Reputation => _reputation,
            Stat.Money => _money,
            Stat.Health => _health,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, "Tried to get invalid player stat.")
        };

        /// <summary>
        /// Sets the current value of the specified player's stat.
        /// </summary>
        /// <param name="stat">The player stat to set.</param>
        /// <param name="value">The value to set the player stat to.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the specified stat is invalid.</exception>
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
    }
}