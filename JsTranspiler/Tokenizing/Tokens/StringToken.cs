using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
	public class StringToken : Token<string>, IValueToken
	{
		public StringToken(string value) : base(TokenType.StringData, value)
		{
		}
	}
}
