using System;
using GameSetup;
using Newtonsoft.Json;

namespace CCSS
{
    [Serializable]
    public struct Card
    {
        public Guid ID;
        public ICard.CardType CardType;
        public int NumPeopleInvolved;
        public string Scenario;
        public Choice[] Choices;
        
        [JsonConstructor]
        public Card(Guid id, ICard.CardType cardType, int numPeopleInvolved, string scenario, Choice[] choices)
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
        
        [JsonConstructor]
        public Choice(string choiceText, int compatibilityModifier, StatModifier[] statModifiers, int[] edgeModifiers)
        {
            ChoiceText = choiceText;
            CompatibilityModifier = compatibilityModifier;
            StatModifiers = statModifiers;
            EdgeModifiers = edgeModifiers;
        }
        
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