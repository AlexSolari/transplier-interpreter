using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
    public class Token<TType> : IToken
    {
        public TokenType Type { get; set; }

        public TType Value { get; set; }

        public Token(TokenType type, TType value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type} - {Value}";
        }

        public virtual TType GetValue()
        {
            return Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Token<TType> otherToken)
            {
                return otherToken.Type == Type
                    && otherToken.Value.Equals(Value);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hash = 7;
            hash = hash * 23 + Type.GetHashCode();
            hash = hash * 23 + Value.GetHashCode();

            return hash;
        }
    }

    public enum TokenType
    {
        Number,
        LeftPar,
        RightPar,
        LeftBrace,
        RightBrace,
        Keyword,
        Identifier,
        Operator,
        EOL,
        EOF,
        StringData,
        LeftSqBrace,
        RightSqBrace,
        Definition
    }
}
