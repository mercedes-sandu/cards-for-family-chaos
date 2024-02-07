using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utility;
using Application = UnityEngine.Device.Application;

namespace CCSS
{
    public static class CardLoader
    {
        public static List<Card> AllCards;

        private static Dictionary<string, Card> _cardsDictionary;
        private static readonly string PersistentDataPath = $"{Application.persistentDataPath}";
        private const string PathToCardsJson = "cards.json";
        
        /// <summary>
        /// Loads all cards from cards.json into the cards dictionary.
        /// </summary>
        public static void LoadAllCards()
        {
            if (!File.Exists(Path.Combine(PersistentDataPath, PathToCardsJson)))
            {
                FileCopier.CheckForCopies();
            }

            _cardsDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, Card>>(
                    File.ReadAllText(Path.Combine(PersistentDataPath, PathToCardsJson)));
            FindAllCards();
        }

        /// <summary>
        /// Prints all cards to the console.
        /// </summary>
        public static void PrintAllCards()
        {
            AllCards.ForEach(card => Debug.Log(card.ToString()));
        }
        
        /// <summary>
        /// Populates the AllCards list with all available cards.
        /// </summary>
        private static void FindAllCards()
        {
            AllCards = new List<Card>();
            foreach (var card in _cardsDictionary)
            {
                AllCards.Add(card.Value);
            }
        }
        
        // todo: write a function that determines which cards are available to the player based on their progress
    }
}