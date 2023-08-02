using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
	public class OperatorExpression : SingleTokenExpression<OperatorToken>
	{
		public OperatorExpression(OperatorToken token) : base(token, SingleTokenExpressionType.Operator)
		{
		}
	}
}
