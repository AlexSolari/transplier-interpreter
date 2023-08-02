namespace JsTranspiler.Tokenizing.Tokens
{
    public class KeywordToken : Token<Keyword>
    {
        public KeywordToken(string value = "") : base(TokenType.Keyword, GetKeyword(value))
        {
        }

		public KeywordToken(Keyword keyword) : base(TokenType.Keyword, keyword)
		{
		}

		public static Keyword GetKeyword(string value){
            Enum.TryParse(value, ignoreCase: true, out Keyword keyword);

            return keyword;
        }
    }

    public enum Keyword
    {
        Await,
        Break,
        Case,
        Catch,
        Continue,
        Debugger,
        Default,
        Delete,
        Else,
        Export,
        False,
        Finally,
        For,
        If,
        Import,
        In,
        InstanceOf,
        New,
        Null,
        Private,
        Protected,
        Public,
        Return,
        Super,
        Switch,
        Static,
        This,
        Throw,
        Try,
        True,
        TypeOf,
        While,
        With
    }
}
