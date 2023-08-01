using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
    public class ExpressionStack : IEnumerable<ITokenExpression>, ICollection
    {
        private Stack<ITokenExpression> _stack = new Stack<ITokenExpression>();

        public int Count => ((ICollection)_stack).Count;

        public bool IsSynchronized => ((ICollection)_stack).IsSynchronized;

        public object SyncRoot => ((ICollection)_stack).SyncRoot;

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_stack).CopyTo(array, index);
        }

        public IEnumerator<ITokenExpression> GetEnumerator()
        {
            return ((IEnumerable<ITokenExpression>)_stack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_stack).GetEnumerator();
        }

        public void Push(ITokenExpression expression)
        {
            if (expression is EOLExpression
                && _stack.Count > 0
                && _stack.Peek() is EOLExpression)
            {
                return;
            }

            _stack.Push(expression);
        }

        public ITokenExpression Pop()
        {
            return _stack.Pop();
        }
    }
}
