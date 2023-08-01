using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class UnaryExpression<TToken> : ITokenExpression
        where TToken : IToken
    {
        public ITokenExpression Arg1 { get; set; }
        public TToken Operator { get; set; }

        public UnaryExpression(ITokenExpression arg1, TToken @operator)
        {
            Arg1 = arg1;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Operator} {Arg1}\n";
        }
    }
}
