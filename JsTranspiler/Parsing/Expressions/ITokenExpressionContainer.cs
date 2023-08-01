namespace JsTranspiler.Parsing.Expressions
{
    public interface ITokenExpressionContainer : ITokenExpression
    {
        IEnumerable<ITokenExpression> Expressions { get; }
    }
}
