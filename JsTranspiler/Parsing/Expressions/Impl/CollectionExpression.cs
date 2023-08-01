﻿namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class CollectionExpression : ITokenExpression, ITokenExpressionContainer
    {
        public IEnumerable<ITokenExpression> Expressions { get; set; }

        public CollectionExpression(IEnumerable<ITokenExpression> expressions)
        {
            Expressions = expressions;
        }

        public override string ToString()
        {
            return $"[ {string.Join(", ", Expressions.Select(x => x.ToString()))} ]";
        }
    }
}