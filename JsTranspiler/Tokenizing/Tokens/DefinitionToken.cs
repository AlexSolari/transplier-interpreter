namespace JsTranspiler.Tokenizing.Tokens
{
    public class DefinitionToken : Token<string>
    {
        public DefinitionToken(string value = "") : base(TokenType.Definition, value)
        {
        }

        public static DefinitionToken Method
        {
            get
            {
                return new DefinitionToken("function");
            }
        }
    }
}
