namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class IndexerExpression : ITokenExpression, ITokenExpressionContainer
    {
        public ITokenExpression Container { get; set; }
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public IndexerExpression(ITokenExpression container, IEnumerable<ITokenExpression> expressions)
        {
            Expressions = expressions;
            Container = container;
        }

        public override string ToString()
        {
            return $"{Container}[{string.Join(", ", Expressions.Select(x => x.ToString()))}]";
        }
    }
}
