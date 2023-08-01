using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class UnaryExpression : ITokenExpression
    {
        ITokenExpression Arg1 { get; set; }
        Token Operator { get; set; }

        public UnaryExpression(ITokenExpression arg1, Token @operator)
        {
            Arg1 = arg1;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Operator.Value} {Arg1}\n";
        }
    }
}
