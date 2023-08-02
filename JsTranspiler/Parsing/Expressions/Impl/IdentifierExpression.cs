using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class IdentifierExpression : SingleTokenExpression<IdentifierToken>, IValueExpression
	{
		public IdentifierExpression(IdentifierToken token) : base(token, SingleTokenExpressionType.Identifier)
		{
		}

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
        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is IdentifierExpression identifier)
            {
                return Token.Equals(identifier.Token);
            }

            return base.Equals(obj);
        }

        public static bool operator==(IdentifierExpression left, IdentifierExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(IdentifierExpression left, IdentifierExpression right)
        {
            return !left.Equals(right);
        }
    }
}
