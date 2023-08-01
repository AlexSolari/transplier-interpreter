namespace JsTranspiler.Tokenizing.Tokens
{
    public class NumberToken : Token<int>
    {
        public NumberToken(string value = "") : base(TokenType.Number, int.Parse(value))
        {
        }

        public NumberToken(int value) : base(TokenType.Number, value)
        {

        }
    }
}
