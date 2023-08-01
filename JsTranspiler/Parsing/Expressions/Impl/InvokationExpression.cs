namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class InvokationExpression : ITokenExpression, ITokenExpressionContainer
    {
        public ITokenExpression Action { get; set; }
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public InvokationExpression(ITokenExpression action, IEnumerable<ITokenExpression> arguments)
        {
            Action = action;
            Expressions = arguments;
        }
        public InvokationExpression(ITokenExpression action, ITokenExpressionContainer arguments)
        {
            Action = action;
            Expressions = arguments.Expressions;
        }

        public override string ToString()
        {
            return $"Invokation {Action}({string.Join(", ", Expressions.Select(x => x.ToString()))})\n";
        }
    }
}
