﻿using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class DefinitionExpression : ITokenExpression, IValueExpression
    {
        public DefinitionToken Definition { get; set; }
        public IdentifierToken Identifier { get; set; }
        public ITokenExpressionContainer Value { get; set; }

        public IValueExpression this[IdentifierToken identifier] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IValueExpression this[IValueExpression identifier] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DefinitionExpression(DefinitionToken definition, IdentifierToken identifier, ITokenExpressionContainer value)
        {
            Definition = definition;
            Identifier = identifier;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Definition} {Identifier} {Value}\n";
        }
    }
}
