using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
	public class EmptyToken : Token<string>
	{
		public EmptyToken(TokenType type) : base(type, string.Empty)
		{
		}
	}
}
