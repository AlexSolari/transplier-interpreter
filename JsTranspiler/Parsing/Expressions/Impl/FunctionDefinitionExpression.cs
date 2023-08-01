using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class FunctionDefinitionExpression : DefinitionExpression
    {
        ITokenExpressionContainer Arguments { get; set; }

        public FunctionDefinitionExpression(Token definition,
            Token identifier,
            ITokenExpressionContainer value,
            ITokenExpressionContainer arguments)
        : base(definition, identifier, value)
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Definition.Value} {Identifier.Value}({Arguments}) {Value}\n";
        }
    }
}
