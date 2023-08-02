using JsTranspiler.Parsing.Expressions;
using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Interpreter
{
    public class Scope
    {
        public Dictionary<IdentifierToken, ITokenExpression> PrimitiveObjects = new();
        public Dictionary<IdentifierToken, Dictionary<IdentifierToken, ITokenExpression>> CompositeObjects = new();

        public Scope()
        {
            
        }

        public Scope(Dictionary<IdentifierToken, ITokenExpression> primitives, Dictionary<IdentifierToken, Dictionary<IdentifierToken, ITokenExpression>> composites)
        {
            PrimitiveObjects = primitives;
            CompositeObjects = composites;
        }

        public void Add(IdentifierToken identifier, ITokenExpression value)
        {
            PrimitiveObjects[identifier] = value;
        }

        public void Add(IdentifierToken identifier, Dictionary<IdentifierToken, ITokenExpression> value)
        {
            CompositeObjects[identifier] = value;
        }

        internal bool ContainsKey(IdentifierToken token)
        {
            return PrimitiveObjects.ContainsKey(token);
        }

        internal void TryGetValue(IdentifierToken indentifier, out ITokenExpression value)
        {
            PrimitiveObjects.TryGetValue(indentifier, out value);
        }

        public Scope MergeWith(Scope scope)
        {
            var result = new Scope();

            foreach (var key in PrimitiveObjects.Keys)
            {
                result.Add(key, PrimitiveObjects[key]);
            }
            foreach (var key in scope.PrimitiveObjects.Keys)
            {
                if (!result.ContainsKey(key))
                    result.Add(key, scope.PrimitiveObjects[key]);
            }

            foreach (var key in CompositeObjects.Keys)
            {
                result.Add(key, CompositeObjects[key]);
            }
            foreach (var key in scope.CompositeObjects.Keys)
            {
                if (!result.ContainsKey(key))
                    result.Add(key, scope.CompositeObjects[key]);
            }

            return result;
        }

        public ITokenExpression this[IdentifierToken identifierToken]
        {
            get 
            { 
                return PrimitiveObjects[identifierToken]; 
            }
            set
            {
                PrimitiveObjects[identifierToken] = value;
            }
        }
    }
}
