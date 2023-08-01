using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class BinaryExpression<TToken> : ITokenExpression
        where TToken : IToken
    {
        public ITokenExpression Arg1 { get; set; }
        public ITokenExpression Arg2 { get; set; }
        public TToken Operator { get; set; }

        public BinaryExpression(ITokenExpression arg1, ITokenExpression arg2, TToken @operator)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Arg1} {Operator} {Arg2}\n";
        }
    }
}
