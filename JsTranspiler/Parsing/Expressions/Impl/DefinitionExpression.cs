using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class DefinitionExpression : ITokenExpression
    {
        protected Token Definition { get; set; }
        protected Token Identifier { get; set; }
        protected ITokenExpressionContainer Value { get; set; }

        public DefinitionExpression(Token definition, Token identifier, ITokenExpressionContainer value)
        {
            Definition = definition;
            Identifier = identifier;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Definition.Value} {Identifier.Value} {Value}\n";
        }
    }
}
