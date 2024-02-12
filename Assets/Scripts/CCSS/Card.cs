using System;
using GameSetup;
using Newtonsoft.Json;

namespace CCSS
{
    [Serializable]
    public struct Card
    {
        public Guid ID;
        public Type CardType;
        public int NumPeopleInvolved;
        public string Scenario;
        public Choice[] Choices;

        public enum Type
        {
            Choice,
            Event
        }

        [JsonConstructor]
        public Card(Guid id, Type cardType, int numPeopleInvolved, string scenario, Choice[] choices)
        {
            ID = id;
            CardType = cardType;
            NumPeopleInvolved = numPeopleInvolved;
            Scenario = scenario;
            Choices = choices;
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public struct Choice
    {
        public string ChoiceText;
        public int CompatibilityModifier;
        public StatModifier[] StatModifiers;
        public int[] EdgeModifiers;
        public string FollowupCard;

        [JsonConstructor]
        public Choice(string choiceText, int compatibilityModifier, StatModifier[] statModifiers, int[] edgeModifiers,
            string followupCard)
        {
            ChoiceText = choiceText;
            CompatibilityModifier = compatibilityModifier;
            StatModifiers = statModifiers;
            EdgeModifiers = edgeModifiers;
            FollowupCard = followupCard;
        }

        /// <summary>
        /// Creates a choice corresponding to null.
        /// </summary>
        /// <returns>The null choice.</returns>
        public static Choice NullChoice() =>
            new("", int.MaxValue, Array.Empty<StatModifier>(), Array.Empty<int>(), "null");

        /// <summary>
        /// Returns whether the choice has a followup card that will be presented to the player once the choice is made.
        /// </summary>
        /// <returns>True if there is a followup card (has a number string), false otherwise.</returns>
        public bool HasFollowup() => !FollowupCard.Equals("") && !FollowupCard.Equals("null");
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public struct StatModifier
    {
        public Player.Stat Stat;
        public int Value;

        [JsonConstructor]
        public StatModifier(Player.Stat stat, int value)
        {
            Stat = stat;
            Value = value;
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}