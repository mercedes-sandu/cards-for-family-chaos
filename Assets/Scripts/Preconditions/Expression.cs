using System.Linq;
using CCSS;

namespace Preconditions
{
    public abstract class Expression<T>
    {
        public abstract T Evaluate(Card card);
    }
    
    public class Constant<T> : Expression<T>
    {
        private readonly T _value;

        public Constant(T value)
        {
            _value = value;
        }

        public override T Evaluate(Card card)
        {
            return _value;
        }
        
        public static implicit operator Constant<T>(T value) => new (value);
    }

    public class Variable<T> : Expression<T>
    {
        private readonly string _name;

        public Variable(string name)
        {
            _name = name;
        }

        public override T Evaluate(Card card)
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class Precondition : Expression<bool>
    {
        public static And operator &(Precondition left, Precondition right) => new (left, right);
        
        public static Or operator |(Precondition left, Precondition right) => new (left, right);
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