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
	}
}
