using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class CollectionExpression : ITokenExpression, ITokenExpressionContainer, IValueExpression
	{
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public IValueExpression this[IValueExpression identifier]
        {
            get => throw new InvalidOperationException(string.Format(IValueExpression.AccessError, identifier));
            set => throw new InvalidOperationException(string.Format(IValueExpression.AccessError, identifier));
        }
        public IValueExpression this[IdentifierToken identifier]
        {
            get => throw new InvalidOperationException(string.Format(IValueExpression.AccessError, identifier));
            set => throw new InvalidOperationException(string.Format(IValueExpression.AccessError, identifier));
        }
        public CollectionExpression(IEnumerable<ITokenExpression> expressions)
        {
            Expressions = expressions;
        }

        public override string ToString()
        {
            return $"[ {string.Join(", ", Expressions.Select(x => x.ToString()))} ]";
        }
    }
}
