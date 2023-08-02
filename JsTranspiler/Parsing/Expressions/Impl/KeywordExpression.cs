using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
	public class KeywordExpression : SingleTokenExpression<KeywordToken>
	{
		public KeywordExpression(KeywordToken token) : base(token, SingleTokenExpressionType.Keyword)
		{
		}
	}
}
