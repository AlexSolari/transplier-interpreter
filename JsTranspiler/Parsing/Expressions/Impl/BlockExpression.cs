namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class BlockExpression : ITokenExpression, ITokenExpressionContainer
    {
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public BlockExpression(IEnumerable<ITokenExpression> expressions)
        {
            Expressions = expressions;
        }

        public override string ToString()
        {
            return $$"""{ {{string.Join(", ", Expressions.Select(x => x.ToString()))}} }""" + "\n";
        }
    }
}
