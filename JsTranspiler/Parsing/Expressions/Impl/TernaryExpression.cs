using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class TernaryExpression<TToken> : ITokenExpression
        where TToken : IToken
    {
        ITokenExpression Arg1 { get; set; }
        ITokenExpression Arg2 { get; set; }
        ITokenExpression Arg3 { get; set; }
        TToken Operator { get; set; }

        public TernaryExpression(ITokenExpression arg1, ITokenExpression arg2, ITokenExpression arg3, TToken @operator)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Arg1} {Operator} {Arg2} {Arg3}\n";
        }
    }
}
