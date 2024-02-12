using System.Collections.Generic;
using CCSS;
using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        // todo: figure out how to store game progress data

        public static Card CurrentCard { get; private set; }

        private static int _currentCardIndex; // todo: remove after preconditions implemented
        private static int _weekNumber = 0;
        private static List<Card> _cardsShown; // todo: do i need this?
        
        /// <summary>
        /// Selects the first card to display to the player.
        /// Note: At this point, all cards have been loaded by CardLoader.
        /// </summary>
        private void Start()
        {
            _cardsShown = new List<Card>();
            SelectNewCard(Choice.NullChoice());
        }

        /// <summary>
        /// Chooses a new card to display to the player. Calls the corresponding game event once the card has been
        /// chosen.
        /// </summary>
        public static void SelectNewCard(Choice choice)
        {
            if (choice.HasFollowup())
            {
                CurrentCard = CardLoader.GetCard(choice.FollowupCard);
            }
            else
            {
                // todo: eventually incorporate picking cards by preconditions that are satisfied
                // use function in CardLoader to determine which cards are available to the player
                CurrentCard = CardLoader.AllCards[_currentCardIndex];
                _currentCardIndex++;
            }
            
            _cardsShown.Add(CurrentCard);
            _weekNumber++;
            GameEvent.CardSelected(CurrentCard, _weekNumber);
        }
    }
}