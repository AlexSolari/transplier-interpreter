using JsTranspiler.Parsing.Expressions.Impl;
using JsTranspiler.Parsing.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Utils
{
	public static class GroupExpressionHelper
	{
		public static ITokenExpressionContainer Containerize(this ITokenExpression expression)
		{
			return new GroupExpression(new[] { expression });
		}


		public static ITokenExpression Unwrap(this ITokenExpression expression)
		{
			if (expression is GroupExpression gExp)
			{
				if (gExp.Expressions.Count() == 1)
					return Unwrap(gExp.Expressions.First());
			}

			return expression;
		}
	}
}
