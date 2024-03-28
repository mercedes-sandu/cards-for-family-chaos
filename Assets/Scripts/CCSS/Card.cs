using System.Collections.Generic;
using System.Linq;
using GameSetup;

namespace CCSS
{
    public class Card
    {
        public readonly Dictionary<string, Character> RoleToCharacter;
        public readonly string Scenario;
        public readonly Choice ChoiceOne;
        public readonly Choice ChoiceTwo;

        private readonly CardTemplate _cardTemplate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardTemplate"></param>
        /// <param name="roleToCharacter"></param>
        public Card(CardTemplate cardTemplate, Dictionary<string, Character> roleToCharacter)
        {
            _cardTemplate = cardTemplate;
            RoleToCharacter = roleToCharacter;
            Scenario = ReplaceRolesWithCharacters(cardTemplate.Scenario);
            ChoiceOne = cardTemplate.Choices[0];
            ChoiceTwo = cardTemplate.Choices[1];
            ChoiceOne.ChoiceText = ReplaceRolesWithCharacters(cardTemplate.Choices[0].ChoiceText);
            ChoiceTwo.ChoiceText = ReplaceRolesWithCharacters(cardTemplate.Choices[1].ChoiceText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Character GetRoleCharacter(string roleName) => RoleToCharacter[roleName];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ReplaceRolesWithCharacters(string text) => RoleToCharacter.Aggregate(text,
            (current, pair) => current.Replace(pair.Key, $"{pair.Value.FirstName} {pair.Value.Surname}"));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            $"{Scenario}\nChoice 1: {ChoiceOne.ChoiceText}\nChoice 2: {ChoiceTwo.ChoiceText}";
    }
}