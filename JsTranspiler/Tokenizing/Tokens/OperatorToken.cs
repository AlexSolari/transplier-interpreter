namespace JsTranspiler.Tokenizing.Tokens
{
    public class OperatorToken : Token<Operator>
    {
        public string RawValue;
        public OperatorToken(string value = "") : base(TokenType.Operator, GetOperator(value))
        {
            RawValue = value;
        }

        public static Operator GetOperator(string value)
        {
            switch (value)
            {
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "*":
                    return Operator.Multiplication;
                case "/":
                    return Operator.Division;
                case "%":
                    return Operator.WholeDivision;
                case "=":
                    return Operator.Assign;
                case "==":
                    return Operator.Equals;
                case "!=":
                    return Operator.NotEquals;
                case ".":
                    return Operator.Access;
                case ">":
                    return Operator.More;
                case ">=":
                    return Operator.MoreEquals;
                case "<":
                    return Operator.Less;
                case "<=":
                    return Operator.LessEquals;
                case ",":
                    return Operator.Comma;
                case "&&":
                    return Operator.And;
                case "||":
                    return Operator.Or;
                case "!":
                    return Operator.Not;
                case "=>":
                    return Operator.ArrowFunc;
                case ":":
                    return Operator.JsonAssign;
                case "?":
                    return Operator.Ternary;
                case "++":
                    return Operator.Increment;
                case "--":
                    return Operator.Decrement;
                default:
                    return Operator.Unknown;

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
        Decrement,
        Unknown
	}
}
