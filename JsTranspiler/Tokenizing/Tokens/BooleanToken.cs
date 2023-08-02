using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
	public class BooleanToken : Token<bool>, IValueToken
	{
		public BooleanToken(bool value) : base(TokenType.Boolean, value)
		{
		}
	}
}
