using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions
{
    public interface ISingleTokenExpression
    {
        public IToken Token { get; }
    }
}
