using System.Collections.Generic;
using CCSS;
using UI;
using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        // todo: figure out how to store game progress data
        
        public static Card CurrentCard { get; private set; }

        private static int _currentCardIndex; // todo: remove after preconditions implemented
        private int _weekNumber = 1;
        private static List<Card> _cardsShown;

        /// <summary>
        ///
        /// Note: At this point, all cards have been loaded by CardLoader.
        /// </summary>
        private void Start()
        {
            _cardsShown = new List<Card>();
            SelectNewCard();
        }

        public static void SelectNewCard()
        {
            // todo: eventually incorporate picking cards by preconditions that are satisfied
            // use function in CardLoader to determine which cards are available to the player
            CurrentCard = CardLoader.AllCards[_currentCardIndex];
            _currentCardIndex++;
            _cardsShown.Add(CurrentCard);
            GameEvent.CardSelected(CurrentCard);
        }
    }
}