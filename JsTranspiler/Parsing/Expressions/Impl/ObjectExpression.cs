using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTranspiler.Parsing.Expressions.Impl
{
	public class ObjectExpression : ITokenExpression, ITokenExpressionContainer, IValueExpression
	{
		public Dictionary<IValueExpression, IValueExpression> Values = new();

		public IdentifierToken InstanceOf = new IdentifierToken("Object");

		public ObjectExpression() { }

		public IValueExpression this[IValueExpression identifier]
		{
			get
			{
				return Values[identifier];
			}
			set 
			{ 
				Values[identifier] = value; 
			}
		}

		public IEnumerable<ITokenExpression> Expressions => Values.Values;
	}
}
