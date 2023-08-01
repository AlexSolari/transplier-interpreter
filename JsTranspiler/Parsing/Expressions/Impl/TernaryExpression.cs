using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class TernaryExpression : ITokenExpression
    {
        ITokenExpression Arg1 { get; set; }
        ITokenExpression Arg2 { get; set; }
        ITokenExpression Arg3 { get; set; }
        Token Operator { get; set; }

        public TernaryExpression(ITokenExpression arg1, ITokenExpression arg2, ITokenExpression arg3, Token @operator)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Arg1} {Operator.Value} {Arg2} {Arg3}\n";
        }
    }
}
