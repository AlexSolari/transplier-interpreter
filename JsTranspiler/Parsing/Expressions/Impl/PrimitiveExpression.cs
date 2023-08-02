using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
	public class PrimitiveExpression<TToken> : SingleTokenExpression<TToken>, IValueExpression
		where TToken : IValueToken
	{
		public PrimitiveExpression(TToken token) : base(token, SingleTokenExpressionType.Constant)
		{
		}
	}
}
