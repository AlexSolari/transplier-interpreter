namespace JsTranspiler.Tokenizing.Tokens
{
    public class IdentifierToken : Token<string>
    {
        public IdentifierToken(string value = "") : base(TokenType.Identifier, value)
        {
        }
    }
}
