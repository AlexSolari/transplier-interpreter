using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class SingleTokenExpression : ITokenExpression
    {
        public Token Token { get; set; }

        public SingleTokenExpressionType Type { get; set; }

        public SingleTokenExpression(Token token, SingleTokenExpressionType type)
        {
            Token = token;
            Type = type;
        }

        public override string ToString()
        {
            return Token.Value;
        }
    }

    public enum SingleTokenExpressionType
    {
        Operator,
        Identifier,
        Constant,
        Keyword
    }
}
