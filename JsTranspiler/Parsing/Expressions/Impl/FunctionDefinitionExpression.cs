﻿using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class FunctionDefinitionExpression : DefinitionExpression
    {
        public ITokenExpressionContainer Arguments { get; set; }

        public FunctionDefinitionExpression(DefinitionToken definition,
            IdentifierToken identifier,
            ITokenExpressionContainer value,
            ITokenExpressionContainer arguments)
        : base(definition, identifier, value)
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Definition} {Identifier}({Arguments}) {Value}\n";
        }
    }
}
