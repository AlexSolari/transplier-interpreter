using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
	public class SyntaxToken : Token<string>
	{
		public SyntaxToken(TokenType type, string value) : base(type, value)
		{
		}
	}
}
