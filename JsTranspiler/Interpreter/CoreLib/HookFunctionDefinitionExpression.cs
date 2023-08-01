using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class HookFunctionDefinitionExpression : DefinitionExpression
    {
        public ITokenExpressionContainer Arguments { get; set; }

        public Action<string> Hook { get; set; }

        public HookFunctionDefinitionExpression(DefinitionToken definition,
            IdentifierToken identifier,
            Action<string> value,
            ITokenExpressionContainer arguments)
        : base(definition, identifier, null)
        {
            Arguments = arguments;
            Hook = value;
        }

        public override string ToString()
        {
            return $"{Definition} {Identifier}({Arguments}) {Value}\n";
        }
    }
}
