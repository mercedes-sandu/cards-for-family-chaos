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
        /// Initializes a new Card instance with populated roles.
        /// </summary>
        /// <param name="cardTemplate">The card template being used for this card instance.</param>
        /// <param name="roleToCharacter">A dictionary mapping role string to character that will fill that
        /// role.</param>
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
        /// Returns the character that fills the specified role.
        /// </summary>
        /// <param name="roleName">The role name, for example, "[[X]]".</param>
        /// <returns>The role's filled character.</returns>
        public Character GetRoleCharacter(string roleName) => RoleToCharacter[roleName];

        /// <summary>
        /// Replaces role placeholders in the text with the actual character names.
        /// </summary>
        /// <param name="text">The text to reformat.</param>
        /// <returns>The reformatted text.</returns>
        private string ReplaceRolesWithCharacters(string text) => RoleToCharacter.Aggregate(text,
            (current, pair) => current.Replace(pair.Key, $"{pair.Value.FirstName} {pair.Value.Surname}"));
        
        public override string ToString() =>
            $"{Scenario}\nChoice 1: {ChoiceOne.ChoiceText}\nChoice 2: {ChoiceTwo.ChoiceText}";
    }
}