using System;
using GameSetup;
using Newtonsoft.Json;

namespace CCSS
{
    [Serializable]
    public class CardTemplate
    {
        public Guid ID;
        public int NumRoles; // todo: do i need this?
        public string Scenario;
        public string[] Roles; // todo: update in json

        public Choice[] Choices; // todo: might be choice templates
        // todo: will have list of preconditions.. need subtypes
        // enable Type Name Handling and pass that to the deserializer

        [JsonConstructor]
        public CardTemplate(Guid id, int numRoles, string scenario, string[] roles, Choice[] choices)
        {
            ID = id;
            NumRoles = numRoles;
            Scenario = scenario;
            Roles = roles;
            Choices = choices;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Choice
    {
        public string ChoiceText;
        public int CompatibilityModifier;
        public StatModifier[] StatModifiers;
        public EdgeModifier[] EdgeModifiers;
        public string FollowupCard;

        [JsonConstructor]
        public Choice(string choiceText, int compatibilityModifier, StatModifier[] statModifiers,
            EdgeModifier[] edgeModifiers, string followupCard)
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
            new("", int.MaxValue, Array.Empty<StatModifier>(), Array.Empty<EdgeModifier>(), "null");

        /// <summary>
        /// Returns whether the choice has a followup cardTemplate that will be presented to the player once the choice is made.
        /// </summary>
        /// <returns>True if there is a followup cardTemplate (has a number string), false otherwise.</returns>
        public bool HasFollowup() => !FollowupCard.Equals("") && !FollowupCard.Equals("null");

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class StatModifier
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

    public class EdgeModifier
    {
        public string RoleOne;
        public string RoleTwo;
        public float PositiveModifier;
        public float NegativeModifier;

        [JsonConstructor]
        public EdgeModifier(string roleOne, string roleTwo, float positiveModifier, float negativeModifier)
        {
            RoleOne = roleOne;
            RoleTwo = roleTwo;
            PositiveModifier = positiveModifier;
            NegativeModifier = negativeModifier;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

// array of preconditions
// card instance (template + characters) passes if all preconditions are satisfied
// option 1: precondition is a delegate that returns a boolean (easy to implement, difficult to debug)
// option 2: precondition is an abstract syntax tree (parse tree), different class for each kind of precondition (less
// than, greater than, etc.) 
// base class for preconditions (expressions that return booleans), subclasses for each kind of boolean operator
// base class for everything that returns a number, subclasses for different kinds of expressions that return numbers 
// (affinities, constants, etc.)
// need something to fill in a character (give character that will fill role)
// abstract class Expression<T>, has virtual method Eval that returns T
// preconditions are of type Expression<bool>
// floating point ones are Expression<float>
// characters are Expression<Character>
// operator overloading
// version 0: exhaustive search of all cards with all combinations of characters
// look into O(nlogn) weighted shuffle
// alternatively, random starting point + random prime number over and over
// card is a card template + vector of role-fillers (objects)
// eval method takes Card as an argument
// can use this to implement effects of the card

// preconditions
// affinity of (person a, person b) > x
// have different kinds of affinities
// stat levels, compatibility level
// presence or absence of edges
// stretch goal: characters' likes/dislikes/hobbies/etc.