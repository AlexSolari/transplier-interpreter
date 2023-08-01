using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
    public class Token
    {
        public TokenType Type { get; set; }

        public string Value { get; set; }

        public Token(TokenType type, string value = "")
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{nameof(Token)} {Type.ToString()} - {Value}";
        }

        public static Token Method
        {
            get
            {
                return new Token(TokenType.Definition, "function");
            }
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
