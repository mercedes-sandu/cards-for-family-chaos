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
        public Choices[] Choices;
        
        [JsonConstructor]
        public Card(Guid id, ICard.CardType cardType, int numPeopleInvolved, string scenario, Choices[] choices)
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

    public struct Choices
    {
        public string ChoiceText;
        public int CompatibilityModifier;
        public StatModifiers[] StatModifiers;
        public int[] EdgeModifiers;
        
        [JsonConstructor]
        public Choices(string choiceText, int compatibilityModifier, StatModifiers[] statModifiers, int[] edgeModifiers)
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

    public struct StatModifiers
    {
        public Player.Stat Stat;
        public int Value;
        
        [JsonConstructor]
        public StatModifiers(Player.Stat stat, int value)
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