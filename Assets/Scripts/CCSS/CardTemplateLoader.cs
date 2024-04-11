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
        /// Loads all card templates from cards.json into the card templates dictionary.
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
        /// Prints all card templates to the console.
        /// </summary>
        public static void PrintAllCardTemplates()
        {
            AllCardTemplates.ForEach(card => Debug.Log(card.ToString()));
        }
        
        /// <summary>
        /// Populates the AllCardTemplates list with all available card templates.
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
        /// Returns the card template corresponding to the specified string key.
        /// </summary>
        /// <param name="key">The card template number as a string.</param>
        /// <returns>The card template with the specified number.</returns>
        public static CardTemplate GetCardTemplate(string key) => _cardTemplatesDictionary[key];
    }
}