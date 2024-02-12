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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public static Choice NullChoice() =>
            new("", int.MaxValue, Array.Empty<StatModifier>(), Array.Empty<int>(), "null");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasFollowup() => !FollowupCard.Equals("") && !FollowupCard.Equals("null");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}