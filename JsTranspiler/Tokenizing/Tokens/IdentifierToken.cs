namespace JsTranspiler.Tokenizing.Tokens
{
    public class IdentifierToken : Token<string>, IValueToken
    {
        public IdentifierToken(string value = "") : base(TokenType.Identifier, value)
        {
        }
    }
}
