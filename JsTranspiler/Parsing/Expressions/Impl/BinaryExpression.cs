using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class BinaryExpression : ITokenExpression
    {
        ITokenExpression Arg1 { get; set; }
        ITokenExpression Arg2 { get; set; }
        Token Operator { get; set; }

        public BinaryExpression(ITokenExpression arg1, ITokenExpression arg2, Token @operator)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{Arg1} {Operator.Value} {Arg2}\n";
        }
    }
}
