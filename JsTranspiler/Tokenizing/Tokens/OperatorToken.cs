namespace JsTranspiler.Tokenizing.Tokens
{
    public class OperatorToken : Token
    {
        public Operator Operator { get; set; }

        public OperatorToken(TokenType type, string value = "") : base(type, value)
        {
            switch (value)
            {
                case "+":
                    Operator = Operator.Plus;
                    break;
                case "-":
                    Operator = Operator.Minus;
                    break;
                case "*":
                    Operator = Operator.Multiplication;
                    break;
                case "/":
                    Operator = Operator.Division;
                    break;
                case "%":
                    Operator = Operator.WholeDivision;
                    break;
                case "=":
                    Operator = Operator.Assign;
                    break;
                case "==":
                    Operator = Operator.Equals;
                    break;
                case "!=":
                    Operator = Operator.NotEquals;
                    break;
                case ".":
                    Operator = Operator.Access;
                    break;
                case ">":
                    Operator = Operator.More;
                    break;
                case ">=":
                    Operator = Operator.MoreEquals;
                    break;
                case "<":
                    Operator = Operator.Less;
                    break;
                case "<=":
                    Operator = Operator.LessEquals;
                    break;
                case ",":
                    Operator = Operator.Comma;
                    break;
                case "&&":
                    Operator = Operator.And;
                    break;
                case "||":
                    Operator = Operator.Or;
                    break;
                case "!":
                    Operator = Operator.Not;
                    break;
                case "=>":
                    Operator = Operator.ArrowFunc;
                    break;
                case ":":
                    Operator = Operator.JsonAssign;
                    break;
                case "?":
                    Operator = Operator.Ternary;
                    break;
				case "++":
					Operator = Operator.Increment;
					break;
				case "--":
					Operator = Operator.Decrement;
					break;
				default:
                    break;
            }
        }
    }

    public enum Operator
    {
        Plus,
        Minus,
        Multiplication,
        Division,
        WholeDivision,
        Assign,
        Equals,
        NotEquals,
        Access,
        More,
        MoreEquals,
        Less,
        LessEquals,
        Comma,
        And,
        Or,
        Not,
        ArrowFunc,
        JsonAssign,
		Ternary,
        Increment,
        Decrement
	}
}
