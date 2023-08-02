using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions
{
	public interface IValueExpression : ITokenExpression
    {
        public IValueExpression this[IValueExpression identifier]
        {
            get; set;
        }

        public IValueExpression this[IdentifierToken identifier]
        {
            get; set;
        }
    }
}
