using System.Collections.Generic;
using System.Linq;
using CCSS;
using GameSetup;
using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        public static InGameGraph InGameGraph;

        public static Card CurrentCard { get; private set; }

        public static int CurrentCompatibility { get; private set; }

        [SerializeField] private int minStartCompatibility = 5;
        [SerializeField] private int maxStartCompatibility = 20;

        private static int _currentCardIndex = 0; // todo: remove after preconditions implemented
        private static int _weekNumber = 0;
        private static Character[] _allCharacters;
        private static List<Card> _allPossibleCards;
        private static List<Card> _cardsShown; // todo: do i need this?

        /// <summary>
        /// Randomly sets the starting compatibility value between the two families.
        /// </summary>
        private void Awake()
        {
            CurrentCompatibility = Random.Range(minStartCompatibility, maxStartCompatibility);
        }
        
        /// <summary>
        /// Selects the first cardTemplate to display to the player.
        /// Note: At this point, all cards have been loaded by CardTemplateLoader.
        /// </summary>
        private void Start()
        {
            InGameGraph = new InGameGraph(SetupMaster.CombinedFamily);
            _cardsShown = new List<Card>();
            _allPossibleCards = new List<Card>();
            _allCharacters = InGameGraph.AllCharacters.ToArray();
                
            foreach (CardTemplate cardTemplate in CardTemplateLoader.AllCardTemplates)
            {
                Character[] currentCombination = new Character[cardTemplate.NumRoles];
                GenerateAllPossibleCardsForTemplate(cardTemplate, currentCombination, 0, _allCharacters.Length - 1, 0);
            }

            Debug.Log($"generated {_allPossibleCards.Count} cards");

            SelectNewCard(Choice.NullChoice());
        }

        /// <summary>
        /// Recursively generates all possible cards (with filled character roles) for a given cardTemplate.
        /// </summary>
        /// <param name="cardTemplate">The card template to be filled.</param>
        /// <param name="currentCombination">The current combination of characters to fill the roles.</param>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <param name="index">The current index.</param>
        private void GenerateAllPossibleCardsForTemplate(CardTemplate cardTemplate, Character[] currentCombination,
            int start, int end, int index)
        {
            if (index == cardTemplate.NumRoles)
            {
                Dictionary<string, Character> roleToCharacter = new Dictionary<string, Character>();
                for (int i = 0; i < cardTemplate.NumRoles; i++)
                {
                    roleToCharacter.TryAdd(cardTemplate.Roles[i], currentCombination[i]);
                }
                roleToCharacter.TryAdd(Player.PlayerRole, Player.PlayerCharacter);

                Card newCard = new Card(cardTemplate, roleToCharacter);
                _allPossibleCards.Add(newCard);
                return;
            }

            for (int i = start; i <= end && end - i + 1 >= cardTemplate.NumRoles - index; i++)
            {
                currentCombination[index] = _allCharacters[i];
                GenerateAllPossibleCardsForTemplate(cardTemplate, currentCombination, i + 1, end, index + 1);
            }
        }

        /// <summary>
        /// Chooses a new cardTemplate to display to the player. Calls the corresponding game event once the
        /// cardTemplate has been chosen.
        /// </summary>
        /// <param name="choice"></param>
        public static void SelectNewCard(Choice choice)
        {
            if (choice.HasFollowup())
            {
                Dictionary<string, Character> roleToCharacter = CurrentCard.RoleToCharacter;
                CurrentCard = new Card(CardTemplateLoader.GetCardTemplate(choice.FollowupCard), roleToCharacter);
            }
            else
            {
                // todo: eventually incorporate picking cards by preconditions that are satisfied
                CurrentCard = _allPossibleCards[_currentCardIndex];
                _currentCardIndex++;
            }

            _cardsShown.Add(CurrentCard);
            _weekNumber++;
            GameEvent.CardSelected(CurrentCard, _weekNumber);
        }
    }
}