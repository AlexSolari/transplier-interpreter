namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class GroupExpression : ITokenExpression, ITokenExpressionContainer
    {
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public GroupExpression(IEnumerable<ITokenExpression> expressions)
        {
            Expressions = expressions;
        }

        public override string ToString()
        {
            return $"( {string.Join(", ", Expressions.Select(x => x.ToString()))} )\n";
        }
    }
}
