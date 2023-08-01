using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsTranspiler.Tokenizing.Tokens;
using JsTranspiler.Utils;

namespace JsTranspiler.Tokenizing
{
    public class Tokenizer
	{
		public string[] Definitions = new string[]
		{
			"class",
			"function",
			"const",
			"let",
			"var",
		};

		public string[] Keywords = new string[]
		{
			"await",
			"break",
			"case",
			"catch",
			"continue",
			"debugger",
			"default",
			"delete",
			"else",
			"export",
			"false",
			"finally",
			"for",
			"if",
			"import",
			"in",
			"instanceof",
			"new",
			"null",
			"private",
			"protected",
			"public",
			"return",
			"super",
			"switch",
			"static",
			"this",
			"throw",
			"try",
			"true",
			"typeof",
			"while",
			"with",
		};

		public string[] Operators = new string[]
		{
			"+",
			"-",
			"*",
			"/",
			"%",
			"=",
			".",
			">",
			"<",
			"!",
			",",
			"&",
			"|",
			":",
			"?"
		};

		private bool isCollectingString = false;

		public IEnumerable<IToken> Tokenize(string data)
		{
			var result = new List<IToken>();
			var currentString = string.Empty;

			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == '/' && data[i+1] == '/')
				{
					while (data[i] != '\n')
					{
						i++;
					}
				}

				if (data[i] != '\n' && char.IsWhiteSpace(data[i]))
				{
					if (isCollectingString)
					{
						currentString += data[i];
					}
					continue;
				}

				currentString += data[i];

				if (isCollectingString && !IsStringDelimiter(data[i]))
					continue;

				var peek = (i + 1) < data.Length ? data[i + 1] : char.MinValue;

				// Parse number
				if (int.TryParse(currentString, out var val))
				{
					if (!char.IsNumber(peek))
					{
						result.Add(new NumberToken(currentString));
						currentString = string.Empty;

						continue;
					}
				}

				// Parse parenthesis
				if (currentString.Equals("(") || currentString.Equals(")"))
				{
					var tokenType = currentString.Equals("(")
						? TokenType.LeftPar
						: TokenType.RightPar;

					result.Add(new Token<string>(tokenType, currentString));
					currentString = string.Empty;

					continue;
				}

				// Parse braces
				if (currentString.Equals("{") || currentString.Equals("}"))
				{
					var tokenType = currentString.Equals("{")
						? TokenType.LeftBrace
						: TokenType.RightBrace;

					result.Add(new Token<string>(tokenType, currentString));
					currentString = string.Empty;

					continue;
				}

				// Parse sq braces
				if (currentString.Equals("[") || currentString.Equals("]"))
				{
					var tokenType = currentString.Equals("[")
						? TokenType.LeftSqBrace
						: TokenType.RightSqBrace;

					result.Add(new Token<string>(tokenType, currentString));
					currentString = string.Empty;

					continue;
				}

				// Parse string delimiter
				if (IsStringDelimiter(data[i]))
				{
					if (isCollectingString)
					{
						result.Add(new Token<string>(TokenType.StringData, string.Join("", currentString.SkipLast(1))));
						currentString = currentString.Last().ToString();
					}

					currentString = string.Empty;
					isCollectingString = !isCollectingString;

					continue;
				}

				// Parse keywords & identifiers
				if (currentString.IsAlphaNumeric() && !peek.IsAlphaNumeric())
				{
					var tokenType = IsKeyword(currentString)
						? TokenType.Keyword
						: IsDefinition(currentString)
							? TokenType.Definition
							: TokenType.Identifier;

					if (tokenType == TokenType.Keyword)
					{
						if (currentString.Equals("this")){
							result.Add(new IdentifierToken(currentString));
						}
						else{
							result.Add(new KeywordToken(currentString));
						}

					}
					else
					{
						if (tokenType == TokenType.Definition)
						{
							result.Add(new DefinitionToken(currentString));
						}
						else
						{
                            result.Add(new IdentifierToken(currentString));
                        }
					}

					currentString = string.Empty;

					continue;
				}

				// Parse operators
				if (isOperator(currentString))
				{
					result.Add(new OperatorToken(currentString));
					currentString = string.Empty;

					continue;
				}


				// Parse EOL
				if (currentString.Equals(";") || currentString.Equals("\n"))
				{
					if (currentString.Equals(";"))
					{
						result.Add(new Token<string>(TokenType.EOL, string.Empty));
					}
					currentString = string.Empty;

					continue;
				}
			}

			var refinedResult = new List<IToken>();
			for (int i = 0; i < result.Count; i++)
			{
				var curr = result[i];

				if (i + 1 == result.Count)
				{
					refinedResult.Add(curr);
					break;
				}

				var peek = result[i + 1];

				if (curr is OperatorToken currOp && peek is OperatorToken peekOp)
				{
					refinedResult.Add(new OperatorToken(currOp.RawValue + peekOp.RawValue));
					i += 1;
				}
				else
				{
					refinedResult.Add(curr);
				}
			}

			refinedResult.Add(new Token<string>(TokenType.EOL, string.Empty));
			refinedResult.Add(new Token<string>(TokenType.EOF, string.Empty));

			return refinedResult;
		}

		private bool isOperator(string currentString)
		{
			return Operators.Any(x => x.Equals(currentString.ToLower()));
		}
		private bool IsDefinition(string currentString)
		{
			return Definitions.Any(x => x.Equals(currentString.ToLower()));
		}

		private bool IsKeyword(string currentString)
		{
			return Keywords.Any(x => x.Equals(currentString.ToLower()));
		}

		private bool IsStringDelimiter(char currentChar)
		{
			return currentChar.Equals('"') || currentChar.Equals('\'') || currentChar.Equals('`');
		}
	}
}
