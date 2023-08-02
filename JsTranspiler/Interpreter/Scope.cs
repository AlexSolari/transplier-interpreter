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
        public Dictionary<IdentifierToken, IValueExpression> PrimitiveObjects = new();

        public Scope()
        {
            
        }

        public Scope(Dictionary<IdentifierToken, IValueExpression> primitives)
        {
            PrimitiveObjects = primitives;
        }

        public void Add(IdentifierToken identifier, IValueExpression value)
        {
            PrimitiveObjects[identifier] = value;
        }

        internal bool ContainsKey(IdentifierToken token)
        {
            return PrimitiveObjects.ContainsKey(token);
        }

        internal void TryGetValue(IdentifierToken indentifier, out IValueExpression value)
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

            return result;
        }

        public IValueExpression this[IdentifierToken identifierToken]
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
