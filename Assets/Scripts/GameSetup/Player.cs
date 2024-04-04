using CCSS;
using UI;
using UnityEngine;
using Utility;

namespace GameSetup
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private StatBar reputationBar;
        [SerializeField] private StatBar moneyBar;
        [SerializeField] private StatBar healthBar;

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
            _reputation = reputationBar.GetValue();
            _money = moneyBar.GetValue();
            _health = healthBar.GetValue();
            
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
        /// <returns></returns>
        public static int GetReputation() => _reputation;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetMoney() => _money;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetHealth() => _health;
        
        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            GameEvent.OnChoiceMade -= UpdateStats;
        }
    }
}