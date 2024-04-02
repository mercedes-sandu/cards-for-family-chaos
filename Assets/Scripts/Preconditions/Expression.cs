using System.Linq;
using CCSS;
using GameSetup;
using Utility;

namespace Preconditions
{
    public abstract class Expression<T>
    {
        public abstract T Evaluate(Card card);
    }

    public class FloatConstant : FloatExpression
    {
        private readonly float _value;

        public FloatConstant(float value)
        {
            _value = value;
        }

        public override float Evaluate(Card card)
        {
            return _value;
        }

        public static implicit operator FloatConstant(float value) => new(value);
    }

    public class Role : CharacterExpression
    {
        private readonly string _name;

        public Role(string name)
        {
            _name = name;
        }

        public override Character Evaluate(Card card)
        {
            return card.RoleToCharacter[_name];
        }
    }

    public abstract class CharacterExpression : Expression<Character>
    {
        public static implicit operator CharacterExpression(string name) => new Role(name);
    }

    public abstract class FloatExpression : Expression<float>
    {
        public static implicit operator FloatExpression(float value) => new FloatConstant(value);

        public static Precondition operator <(FloatExpression left, FloatExpression right) => new LessThan(left, right);

        public static Precondition operator >(FloatExpression left, FloatExpression right) =>
            new GreaterThan(left, right);
    }

    public class HasMet : Precondition
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;

        // new HasMet("[[X]]", "[[Y]]")
        public HasMet(CharacterExpression roleOne, CharacterExpression roleTwo)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
        }

        public override bool Evaluate(Card card)
        {
            return GameManager.InGameGraph.AreConnected(_roleOne.Evaluate(card), _roleTwo.Evaluate(card));
        }
    }

    public class LessThan : Precondition
    {
        private readonly FloatExpression _left;
        private readonly FloatExpression _right;

        public LessThan(FloatExpression left, FloatExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Card card)
        {
            return _left.Evaluate(card) < _right.Evaluate(card);
        }
    }

    public class GreaterThan : Precondition
    {
        private readonly FloatExpression _left;
        private readonly FloatExpression _right;

        public GreaterThan(FloatExpression left, FloatExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Card card)
        {
            return _left.Evaluate(card) > _right.Evaluate(card);
        }
    }

    public class Affinity : FloatExpression
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;

        public Affinity(CharacterExpression roleOne, CharacterExpression roleTwo)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
        }

        public override float Evaluate(Card card)
        {
            return GameManager.InGameGraph.EdgesAndInformation[(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))]
                .AffinityPair.NetAffinity;
        }
    }

    public class Likes : Precondition
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;
        private readonly float _minThreshold;

        // potential todo: instead of float min threshold, may be a FloatExpression (i.e., some character's min threshold to liking other characters)

        public Likes(CharacterExpression roleOne, CharacterExpression roleTwo, float minThreshold = 0)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
            _minThreshold = minThreshold;
        }

        public override bool Evaluate(Card card)
        {
            return GameManager.InGameGraph.EdgesAndInformation[(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))]
                .AffinityPair.NetAffinity > _minThreshold;
        }
    }

    public abstract class Precondition : Expression<bool>
    {
        public static And operator &(Precondition left, Precondition right) => new(left, right);

        public static Or operator |(Precondition left, Precondition right) => new(left, right);
    }

    public class And : Precondition
    {
        private readonly Precondition[] _conjuncts;

        public And(params Precondition[] conjuncts)
        {
            _conjuncts = conjuncts;
        }

        public override bool Evaluate(Card card)
        {
            return _conjuncts.All(conjunct => conjunct.Evaluate(card));
        }
    }

    public class Or : Precondition
    {
        private readonly Precondition[] _disjuncts;

        public Or(params Precondition[] disjuncts)
        {
            _disjuncts = disjuncts;
        }

        public override bool Evaluate(Card card)
        {
            return _disjuncts.Any(disjunct => disjunct.Evaluate(card));
        }
    }
}