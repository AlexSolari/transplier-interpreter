using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class SingleTokenExpression<TToken> : ITokenExpression, ISingleTokenExpression
        where TToken : IToken
    {
        public TToken Token { get; set; }

        public SingleTokenExpressionType Type { get; set; }
        IToken ISingleTokenExpression.Token => Token;

        public SingleTokenExpression(TToken token, SingleTokenExpressionType type)
        {
            Token = token;
            Type = type;
        }

        public override string ToString()
        {
            return Token.ToString();
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
