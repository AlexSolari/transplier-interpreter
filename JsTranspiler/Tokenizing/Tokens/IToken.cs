using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Tokenizing.Tokens
{
    public interface IToken
    {
        public TokenType Type { get; set; }
    }
}
