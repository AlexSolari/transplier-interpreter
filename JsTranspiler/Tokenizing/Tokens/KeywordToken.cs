namespace JsTranspiler.Tokenizing.Tokens
{
    public class KeywordToken : Token
    {
        public Keyword Keyword { get; set; }

        public KeywordToken(TokenType type, string value = "") : base(type, value)
        {
            Enum.TryParse(value, ignoreCase: true, out Keyword keyword);

            Keyword = keyword;
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
