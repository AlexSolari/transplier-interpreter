using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsTranspiler.Parsing.Expressions;
using JsTranspiler.Parsing.Expressions.Impl;
using JsTranspiler.Tokenizing.Tokens;

namespace JsTranspiler.Parsing
{
    public class Parser
	{
		IEnumerable<TokenType> Consumables => ConsumableOpposites.Keys;

		Dictionary<TokenType, TokenType> ConsumableOpposites = new()
		{
			[TokenType.LeftBrace] = TokenType.RightBrace,
			[TokenType.LeftSqBrace] = TokenType.RightSqBrace,
			[TokenType.LeftPar] = TokenType.RightPar,
		};

		List<Token> tokens;
		public int Index { get; set; } = 0;
		public Token Current => tokens[Index];
		public Token? Next => Index + 1 < tokens.Count
			? tokens[Index + 1]
			: null;

		public ExpressionStack Expressions { get; set; } = new ExpressionStack();

		public Parser(IEnumerable<Token> tokens)
		{
			this.tokens = tokens.ToList();
		}

		public void Advance() => Index++;

		public IEnumerable<ITokenExpression> Parse()
		{
			while (Current.Type != TokenType.EOF)
			{
				var exp = ParseNextExpression(Expressions);

				Expressions.Push(exp);

				Advance();
			}

			return Expressions;
		}

		ITokenExpression ParseNextExpression(ExpressionStack frame)
		{
			switch (Current.Type)
			{
				case TokenType.StringData:
				case TokenType.Number:
				case TokenType.Keyword:
					return ParseKeyword(frame);
				case TokenType.Identifier:
					return ParseIdentifier();
				case TokenType.Operator:
					return ParseOperator(frame);
				case TokenType.LeftPar:
				case TokenType.LeftBrace:
				case TokenType.LeftSqBrace:
					return ParseConsumable(frame);
				case TokenType.Definition:
					return ParseDefinition(frame);
				case TokenType.EOL:
					return new EOLExpression();
				default:
					throw new InvalidDataException($"Invalid token {Current}");
			}
		}

		private ITokenExpression ParseKeyword(ExpressionStack frame)
		{
			if (Current is not KeywordToken)
			{
				return new SingleTokenExpression(Current, SingleTokenExpressionType.Constant);
			}

			var keywordToken = Current as KeywordToken;

			switch (keywordToken.Keyword)
			{
				case Keyword.Return:
				case Keyword.Throw:
				case Keyword.Await:
				case Keyword.TypeOf:
				case Keyword.InstanceOf:
				case Keyword.New:
					Advance();
					var returnValue = ParseNextExpression(frame);
					var returnKeywordArgument = new GroupExpression(new[] { returnValue ?? new EOLExpression() });

					return new UnaryExpression(returnKeywordArgument, keywordToken);
				case Keyword.If:
					ITokenExpression result;

					Advance();
					var conditionValue = ParseNextExpression(frame);
					Advance();
					var blockValue = ParseNextExpression(frame);

					result = new BinaryExpression(conditionValue, blockValue, keywordToken);

					AdvanceIfEOL();

					while (Next is KeywordToken nextKeyword
						&& nextKeyword.Keyword == Keyword.Else)
					{
						Advance();
						var elseKeyword = ParseNextExpression(frame);
						Advance();
						var elseblockValue = ParseNextExpression(frame);

						result = new BinaryExpression(result, elseblockValue, nextKeyword);

						AdvanceIfEOL();
					}

					return result;
				case Keyword.Switch:
					Advance();
					var switchOn = ParseNextExpression(frame);
					Advance();
					var switchBody = ParseNextExpression(frame);
					Advance(); // to skip }

					return new BinaryExpression(switchOn, switchBody, keywordToken);
				case Keyword.Case:
				case Keyword.Default:
					var cases = new List<ITokenExpression>();

					var localFrame = new ExpressionStack();
					while (Current is KeywordToken kTok
						&& kTok.Keyword == Keyword.Case)
					{
						Advance();
						var caseOn = ParseNextExpression(frame);
						Advance(); // to skip :
						Advance();
						cases.Add(caseOn);
					}

					if (Current is KeywordToken defKTok
						&& defKTok.Keyword == Keyword.Default)
					{
						Advance();
						Advance(); // to skip :
					}

					while (true)
					{
						var expr = ParseNextExpression(localFrame);
						localFrame.Push(expr);

						if (Current is KeywordToken kTok
							&& kTok.Keyword == Keyword.Break)
						{
							break;
						}

						Advance();
					}


					var normalizedContent = localFrame.Reverse().ToList();
					return new BinaryExpression(
						new GroupExpression(cases),
						new BlockExpression(normalizedContent),
						keywordToken);
				case Keyword.Break:
				case Keyword.Catch:
				case Keyword.Continue:
				case Keyword.Debugger:
				case Keyword.Delete:
				case Keyword.Else:
				case Keyword.Export:
				case Keyword.False:
				case Keyword.Finally:
				case Keyword.For:
				case Keyword.Import:
				case Keyword.In:
				case Keyword.Null:
				case Keyword.Private:
				case Keyword.Protected:
				case Keyword.Public:
				case Keyword.Super:
				case Keyword.Static:
				case Keyword.This:
				case Keyword.Try:
				case Keyword.True:
				case Keyword.While:
				case Keyword.With:
				default:
					return new SingleTokenExpression(Current, SingleTokenExpressionType.Keyword);
			};

			void AdvanceIfEOL()
			{
				var afterNext = Index + 2 < tokens.Count
					? tokens[Index + 2]
					: null;

				if (Next?.Type == TokenType.EOL
					&& afterNext is KeywordToken afterNextKeyword
					&& afterNextKeyword.Keyword == Keyword.Else)
				{
					Advance();
				}
			}

		}

		private ITokenExpression ParseIdentifier()
		{
			var frame = new ExpressionStack();

			while (!(Current.Type == TokenType.EOL || Current.Type == TokenType.EOF))
			{

				if (Current.Type == TokenType.Operator && (Current as OperatorToken)?.Operator == Operator.Access)
				{
					var @operator = ParseOperator(frame);
					frame.Push(@operator);
				}
				else if (Current.Type == TokenType.LeftSqBrace || Current.Type == TokenType.LeftPar)
				{
					var indexer = ParseConsumable(frame);
					frame.Push(indexer);
				}
				else if (Current.Type == TokenType.Identifier)
				{
					var current = new SingleTokenExpression(Current, SingleTokenExpressionType.Identifier);
					frame.Push(current);
				}


				if ((Next?.Type == TokenType.Operator && (Next as OperatorToken)?.Operator == Operator.Access)
						|| Next?.Type == TokenType.LeftSqBrace
						|| Next?.Type == TokenType.LeftPar)
				{

					Advance();
				}
				else
				{
					if (frame.Count == 2
						&& frame.Last() is SingleTokenExpression stExp
						&& stExp.Type == SingleTokenExpressionType.Identifier
						&& frame.First() is GroupExpression gExp)
					{
						if (Next?.Type == TokenType.LeftBrace)
						{
							Advance();
							var methodBody = ParseConsumable(frame);
							var f = new FunctionDefinitionExpression(Token.Method, stExp.Token, methodBody, gExp);
							return f;
						}
						else
						{
							var c = new InvokationExpression(stExp, gExp);
							return c;
						}
					}

					break;
				}

			}

			return new GroupExpression(frame.Reverse().ToList());
		}

		private ITokenExpression ParseOperator(ExpressionStack frame)
		{
			var @operator = Current as OperatorToken;

			if (@operator.Operator == Operator.Not)
			{
				Advance();
				var nextExpr = ParseNextExpression(frame);
				return new UnaryExpression(nextExpr, @operator);
			}
			else if (@operator.Operator == Operator.Comma)
			{
				return new SingleTokenExpression(@operator, SingleTokenExpressionType.Operator);
			}
			else if (@operator.Operator == Operator.Decrement
				|| @operator.Operator == Operator.Increment)
			{
				var arg = frame.Pop();
				return new UnaryExpression(arg, @operator);
			}

			var arg1 = frame.Pop();
			Advance();
			var arg2 = ParseNextExpression(frame);

			if (@operator.Operator == Operator.Ternary)
			{
				Advance(); //to skip ":"
				Advance();
				var arg3 = ParseNextExpression(frame);

				return new TernaryExpression(arg1, arg2, arg3, @operator);
			}

			return new BinaryExpression(arg1, arg2, @operator);
		}

		private TokenType GetOppositeType(TokenType type)
		{
			if (Consumables.Contains(type))
			{
				return ConsumableOpposites[type];
			}

			throw new InvalidDataException("Unexpected consumable start");
		}

		private ITokenExpressionContainer ParseConsumable(ExpressionStack frame)
		{
			var start = Current.Type;
			var end = GetOppositeType(Current.Type);
			Advance();
			return Consume(start, end, frame);
		}

		private ITokenExpression ParseDefinition(ExpressionStack frame)
		{
			var definition = Current;
			Token identifier;
			if (Next.Type == TokenType.Identifier)
			{
				Advance();
				identifier = Current;
			}
			else
			{
				identifier = new Token(TokenType.Identifier, $"anonymous_{definition.Value}_{Guid.NewGuid()}");
			}
			Advance();

			if (Current.Type == TokenType.Operator && Current.Value.Equals("="))
			{
				Advance();
			}

			ITokenExpression? value = null;

			switch (definition.Value)
			{
				case "class":
					value = ParseNextExpression(frame);
					frame.Push(value);
					break;
				case "let":
				case "const":
				case "var":
					var localFrame = new ExpressionStack();
					while (Current.Type != TokenType.EOL)
					{
						value = ParseNextExpression(localFrame);
						localFrame.Push(value);

						Advance();
					}
					break;
				case "function":
					var arguments = ParseNextExpression(frame);
					Advance();
					var body = ParseNextExpression(frame);
					return new FunctionDefinitionExpression(definition,
						identifier,
						body as ITokenExpressionContainer,
						arguments as ITokenExpressionContainer);
				default:
					break;
			}



			var resultValue = value is not ITokenExpressionContainer
					? new GroupExpression(new[] { value ?? new EOLExpression() })
					: (ITokenExpressionContainer)value;

			return new DefinitionExpression(
				definition,
				identifier,
				resultValue);
		}

		private ITokenExpressionContainer Consume(TokenType from, TokenType to, ExpressionStack frame)
		{
			var localFrame = new ExpressionStack();

			while (Current.Type != to)
			{
				if (Consumables.Contains(Current.Type))
				{
					var start = Current.Type;
					var end = GetOppositeType(Current.Type);
					Advance();
					localFrame.Push(Consume(start, end, localFrame));
					Advance();
				}
				else
				{
					var expr = ParseNextExpression(localFrame);

					if (expr != null)
					{
						localFrame.Push(expr);
					}

					Advance();
				}

				if (Current.Type == TokenType.EOL)
				{
					Advance();
				}
			}

			var normalizedContent = localFrame.Reverse().ToList();

			switch (from)
			{
				case TokenType.LeftPar:
					return new GroupExpression(normalizedContent);
				case TokenType.LeftBrace:
					return new BlockExpression(normalizedContent);
				case TokenType.LeftSqBrace:
					return frame.Any() 
						? new IndexerExpression(frame.Pop(), normalizedContent)
						: new CollectionExpression(normalizedContent);
				default:
					throw new InvalidDataException("Unexpected consumable start");
			}
		}
	}
}
