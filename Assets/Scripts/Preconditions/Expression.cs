using System;
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

    public abstract class CharacterExpression : Expression<Character>
    {
        public static implicit operator CharacterExpression(string name) => new Role(name);
    }

    public abstract class IntExpression : Expression<int>
    {
        public static implicit operator IntExpression(int value) => new IntConstant(value);

        // todo: implement <, >, <=, >=, ==, !=
    }

    public abstract class FloatExpression : Expression<float>
    {
        public static implicit operator FloatExpression(float value) => new FloatConstant(value);

        public static Precondition operator <(FloatExpression left, FloatExpression right) => new LessThan(left, right);

        public static Precondition operator >(FloatExpression left, FloatExpression right) =>
            new GreaterThan(left, right);

        // todo: implement <=, >=, ==, !=
        // public static Precondition operator ==(FloatExpression left, FloatExpression right) => new EqualTo(left, right);
        //
        // public static Precondition operator !=(FloatExpression left, FloatExpression right) => !(left == right);
    }

    public abstract class Precondition : Expression<bool>
    {
        public static And operator &(Precondition left, Precondition right) => new(left, right);

        public static Or operator |(Precondition left, Precondition right) => new(left, right);
    }

    public class IntConstant : IntExpression
    {
        private readonly int _value;

        public IntConstant(int value)
        {
            _value = value;
        }

        public override int Evaluate(Card card)
        {
            return _value;
        }

        public static implicit operator IntConstant(int value) => new(value);
    }

    public class Compatibility : IntExpression
    {
        public override int Evaluate(Card card)
        {
            return GameManager.CurrentCompatibility;
        }
    }

    public class Reputation : IntExpression
    {
        public override int Evaluate(Card card)
        {
            return Player.GetPlayerStat(Player.Stat.Reputation);
        }
    }
    
    public class Money : IntExpression
    {
        public override int Evaluate(Card card)
        {
            return Player.GetPlayerStat(Player.Stat.Money);
        }
    }
    
    public class Health : IntExpression
    {
        public override int Evaluate(Card card)
        {
            return Player.GetPlayerStat(Player.Stat.Health);
        }
    }

    public class FamilyCompatibility : IntExpression
    {
        public override int Evaluate(Card card)
        {
            return GameManager.CurrentCompatibility;
        }
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

    public class PositiveAffinity : FloatExpression
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;

        public PositiveAffinity(CharacterExpression roleOne, CharacterExpression roleTwo)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
        }

        public override float Evaluate(Card card)
        {
            return GameManager.InGameGraph.GetAffinityPair(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))
                .PositiveAffinity;
        }
    }

    public class NegativeAffinity : FloatExpression
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;

        public NegativeAffinity(CharacterExpression roleOne, CharacterExpression roleTwo)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
        }

        public override float Evaluate(Card card)
        {
            return GameManager.InGameGraph.GetAffinityPair(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))
                .NegativeAffinity;
        }
    }

    public class NetAffinity : FloatExpression
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;

        public NetAffinity(CharacterExpression roleOne, CharacterExpression roleTwo)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
        }

        public override float Evaluate(Card card)
        {
            return GameManager.InGameGraph.GetAffinityPair(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))
                .NetAffinity;
        }
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

    public class EqualTo : Precondition
    {
        private readonly FloatExpression _left;
        private readonly FloatExpression _right;
        private const float Tolerance = 0.01f;

        public EqualTo(FloatExpression left, FloatExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Card card)
        {
            return Math.Abs(_left.Evaluate(card) - _right.Evaluate(card)) < Tolerance;
        }
    }

    public class Likes : Precondition
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;
        private readonly FloatExpression _minThreshold;

        public Likes(CharacterExpression roleOne, CharacterExpression roleTwo, FloatExpression minThreshold = null)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
            _minThreshold = minThreshold ?? 0;
        }

        public override bool Evaluate(Card card)
        {
            return GameManager.InGameGraph.GetAffinityPair(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))
                .NetAffinity > _minThreshold.Evaluate(card);
        }
    }

    public class Dislikes : Precondition
    {
        private readonly CharacterExpression _roleOne;
        private readonly CharacterExpression _roleTwo;
        private readonly FloatExpression _maxThreshold;

        public Dislikes(CharacterExpression roleOne, CharacterExpression roleTwo, FloatExpression maxThreshold = null)
        {
            _roleOne = roleOne;
            _roleTwo = roleTwo;
            _maxThreshold = maxThreshold ?? 0;
        }

        public override bool Evaluate(Card card)
        {
            return GameManager.InGameGraph.GetAffinityPair(_roleOne.Evaluate(card), _roleTwo.Evaluate(card))
                .NetAffinity < _maxThreshold.Evaluate(card);
        }
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