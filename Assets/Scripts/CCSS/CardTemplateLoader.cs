using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Utility;
using Application = UnityEngine.Device.Application;

namespace CCSS
{
    public static class CardTemplateLoader
    {
        public static List<CardTemplate> AllCardTemplates;

        private static Dictionary<string, CardTemplate> _cardTemplatesDictionary;
        private static readonly string PersistentDataPath = $"{Application.persistentDataPath}";
        private const string PathToCardTemplatesJson = "cards.json";
        
        /// <summary>
        /// Loads all cards from cards.json into the cards dictionary.
        /// </summary>
        public static void LoadAllCardTemplates()
        {
            if (!File.Exists(Path.Combine(PersistentDataPath, PathToCardTemplatesJson)))
            {
                FileCopier.CheckForCopies();
            }

            _cardTemplatesDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, CardTemplate>>(
                    File.ReadAllText(Path.Combine(PersistentDataPath, PathToCardTemplatesJson)));
            FindAllCardTemplates();
        }

        /// <summary>
        /// Prints all cards to the console.
        /// </summary>
        public static void PrintAllCardTemplates()
        {
            AllCardTemplates.ForEach(card => Debug.Log(card.ToString()));
        }
        
        /// <summary>
        /// Populates the AllCardTemplates list with all available cards.
        /// </summary>
        private static void FindAllCardTemplates()
        {
            AllCardTemplates = new List<CardTemplate>();
            foreach (var card in _cardTemplatesDictionary)
            {
                AllCardTemplates.Add(card.Value);
            }
        }

        /// <summary>
        /// Returns the cardTemplate corresponding to the specified string key.
        /// </summary>
        /// <param name="key">The cardTemplate number as a string.</param>
        /// <returns>The cardTemplate with the specified number.</returns>
        public static CardTemplate GetCardTemplate(string key) => _cardTemplatesDictionary[key];
    }
}