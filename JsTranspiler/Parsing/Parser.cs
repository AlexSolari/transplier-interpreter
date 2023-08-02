using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsTranspiler.Parsing.Expressions;
using JsTranspiler.Parsing.Expressions.Impl;
using JsTranspiler.Tokenizing.Tokens;
using JsTranspiler.Utils;

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

		List<IToken> tokens;
		public int Index { get; set; } = 0;
		public IToken Current => tokens[Index];
		public IToken? Next => Index + 1 < tokens.Count
			? tokens[Index + 1]
			: null;

		public ExpressionStack Expressions { get; set; } = new ExpressionStack();

		public Parser(IEnumerable<IToken> tokens)
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
				case TokenType.Boolean:
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
				switch (Current.Type)
				{
					case TokenType.Number:
                        return new PrimitiveExpression<NumberToken>(Current as NumberToken);
                    case TokenType.StringData:
                        return new PrimitiveExpression<StringToken>(Current as StringToken);
					case TokenType.Boolean:
						return new PrimitiveExpression<BooleanToken>(Current as BooleanToken);
					default:
						throw new InvalidDataException($"Invalid token type {Current.Type}");
				}
			}

			var keywordToken = Current as KeywordToken;

			switch (keywordToken.Value)
			{
				case Keyword.Return:
				case Keyword.Throw:
				case Keyword.Await:
				case Keyword.TypeOf:
				case Keyword.InstanceOf:
				case Keyword.New:
					Advance();
					var exps = new ExpressionStack();
					while (Current?.Type != TokenType.EOL)
					{
						var exp = ParseNextExpression(exps);
						exps.Push(exp);

						if (Current?.Type != TokenType.EOL)
						{
							Advance();
						}
					}
					var normalizedContent = exps.Reverse().ToList();
					var returnKeywordArgument = new GroupExpression(normalizedContent);

					return new UnaryExpression<KeywordToken>(returnKeywordArgument, keywordToken);
				case Keyword.If:
					ITokenExpression result;

					Advance();
					var conditionValue = ParseNextExpression(frame);
					Advance();
					var blockValue = ParseNextExpression(frame);

					result = new BinaryExpression<KeywordToken>(conditionValue, blockValue, keywordToken);

					AdvanceIfEOL();

					while (Next is KeywordToken nextKeyword
						&& nextKeyword.Value == Keyword.Else)
					{
						Advance();
						var elseKeyword = ParseNextExpression(frame);
						Advance();
						var elseblockValue = ParseNextExpression(frame);

						result = new BinaryExpression<KeywordToken>(result, elseblockValue, nextKeyword);

						AdvanceIfEOL();
					}

					return result;
				case Keyword.Switch:
					Advance();
					var switchOn = ParseNextExpression(frame);
					Advance();
					var switchBody = ParseNextExpression(frame);
					Advance(); // to skip }

					return new BinaryExpression<KeywordToken>(switchOn, switchBody, keywordToken);
				case Keyword.Case:
				case Keyword.Default:
					var cases = new List<ITokenExpression>();

					var localFrame = new ExpressionStack();
					while (Current is KeywordToken kTok
						&& kTok.Value == Keyword.Case)
					{
						Advance();
						var caseOn = ParseNextExpression(frame);
						Advance(); // to skip :
						Advance();
						cases.Add(caseOn);
					}

					if (Current is KeywordToken defKTok
						&& defKTok.Value == Keyword.Default)
					{
						Advance();
						Advance(); // to skip :
					}

					while (true)
					{
						var expr = ParseNextExpression(localFrame);
						localFrame.Push(expr);

						if (Current is KeywordToken kTok
							&& kTok.Value == Keyword.Break)
						{
							break;
						}

						Advance();
					}


					normalizedContent = localFrame.Reverse().ToList();
					return new BinaryExpression<KeywordToken>(
						new GroupExpression(cases),
						new BlockExpression(normalizedContent),
						keywordToken);
				case Keyword.This:
				case Keyword.Null:
				case Keyword.True:
				case Keyword.False:
					throw new InvalidDataException($"Token {keywordToken} should not be a keyword, but an identifier");
				case Keyword.Break:
				case Keyword.Catch:
				case Keyword.Continue:
				case Keyword.Debugger:
				case Keyword.Delete:
				case Keyword.Else:
				case Keyword.Export:
				case Keyword.Finally:
				case Keyword.For:
				case Keyword.Import:
				case Keyword.In:
				case Keyword.Private:
				case Keyword.Protected:
				case Keyword.Public:
				case Keyword.Super:
				case Keyword.Static:
				case Keyword.Try:
				case Keyword.While:
				case Keyword.With:
				default:
					return new KeywordExpression(keywordToken);
			};

			void AdvanceIfEOL()
			{
				var afterNext = Index + 2 < tokens.Count
					? tokens[Index + 2]
					: null;

				if (Next?.Type == TokenType.EOL
					&& afterNext is KeywordToken afterNextKeyword
					&& afterNextKeyword.Value == Keyword.Else)
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

				if (Current is OperatorToken opTok && opTok.Value == Operator.Access)
				{
					var @operator = ParseOperator(frame);
					frame.Push(@operator);
				}
				else if (Current.Type == TokenType.LeftSqBrace || Current.Type == TokenType.LeftPar)
				{
					var indexer = ParseConsumable(frame);
					frame.Push(indexer);
				}
				else if (Current is IdentifierToken identifierToken)
				{
					var current = new IdentifierExpression(identifierToken);
					frame.Push(current);
				}


				if ((Next?.Type == TokenType.Operator && (Next as OperatorToken)?.Value == Operator.Access)
						|| Next?.Type == TokenType.LeftSqBrace
						|| Next?.Type == TokenType.LeftPar)
				{

					Advance();
				}
				else
				{
					if (frame.Count == 2
						&& frame.Last() is SingleTokenExpression<IdentifierToken> stExp
						&& stExp.Type == SingleTokenExpressionType.Identifier
						&& frame.First() is GroupExpression gExp)
					{
						if (Next?.Type == TokenType.LeftBrace)
						{
							Advance();
							var methodBody = ParseConsumable(frame);
							var argumentsExp = gExp
								.Expressions
								.Where(x => (x as SingleTokenExpression<OperatorToken>)?.Token?.Value != Operator.Comma);
                            var funcDef = new FunctionDefinitionExpression(
								DefinitionToken.Method, 
								stExp.Token, 
								methodBody, 
								new GroupExpression(argumentsExp));
							return funcDef;
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

			if (@operator.Value == Operator.Not)
			{
				Advance();
				var nextExpr = ParseNextExpression(frame);
				return new UnaryExpression<OperatorToken>(nextExpr, @operator);
			}
			else if (@operator.Value == Operator.Comma)
			{
				return new OperatorExpression(@operator);
			}
			else if (@operator.Value == Operator.Decrement
				|| @operator.Value == Operator.Increment)
			{
				var arg = frame.Pop();
				return new UnaryExpression<OperatorToken>(arg, @operator);
			}
			var arg1 = frame.Pop();
			Advance();
			var arg2 = ParseNextExpression(frame);

			if (@operator.Value == Operator.Ternary)
			{
				Advance(); //to skip ":"
				Advance();
				var arg3 = ParseNextExpression(frame);

				return new TernaryExpression<OperatorToken>(arg1, arg2, arg3, @operator);
			}
            else if (@operator.Value == Operator.ArrowFunc)
            {
                var identifier = new IdentifierToken($"anonymous_{@operator.Value}_{Guid.NewGuid()}");
				return new FunctionDefinitionExpression(DefinitionToken.Method,
					identifier,
					new GroupExpression(new[] { arg2 }),
                    new GroupExpression(new[] { arg1 }));
            }

            return new BinaryExpression<OperatorToken>(arg1, arg2, @operator);
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
			var definition = Current as DefinitionToken;
            IdentifierToken identifier;
			if (Next is IdentifierToken identifierToken)
			{
				Advance();
				identifier = identifierToken;
			}
			else
			{
				identifier = new IdentifierToken($"anonymous_{definition.Value}_{Guid.NewGuid()}");
			}
			Advance();

			if (Current is OperatorToken opTok && opTok.Value == Operator.Assign)
			{
				Advance();
			}

			ITokenExpression? value = null;

			switch (definition.Value)
			{
				case "class":
					value = ParseNextExpression(frame);
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
					var argumentsList = (ParseNextExpression(frame) as GroupExpression)
						.Expressions
						.Where(x => (x as SingleTokenExpression<OperatorToken>)?.Token?.Value != Operator.Comma);
					var arguments = new GroupExpression( argumentsList );
					Advance();
					var body = ParseNextExpression(frame);
					return new FunctionDefinitionExpression(definition,
						identifier,
						body as ITokenExpressionContainer,
						arguments);
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
					var hasBinExps = normalizedContent.Where(x => x is BinaryExpression<OperatorToken>).Any();
					var allOperatorsAreCommas = normalizedContent
						.Where(x => x is OperatorExpression)
						.Cast<OperatorExpression>()
						.All(x => x.Token.Value == Operator.Comma);
					var allBinExpsAreJsonAssign = normalizedContent
						.Where(x => x is BinaryExpression<OperatorToken>)
						.Cast<BinaryExpression<OperatorToken>>()
						.All(x => x.Operator.Value == Operator.JsonAssign);
					if (hasBinExps && allOperatorsAreCommas && allBinExpsAreJsonAssign)
					{
						var res = new ObjectExpression();
						foreach (var item in normalizedContent)
						{
							if (item is OperatorExpression)
								continue;

							var binExp = item as BinaryExpression<OperatorToken>;
							var key = binExp.Arg1.Unwrap() as IValueExpression;
							var value = binExp.Arg2.Unwrap() as IValueExpression;

							res[key] = value;
						}

						return res;
					}

					return new BlockExpression(normalizedContent);
				case TokenType.LeftSqBrace:
                    var hasValExps = normalizedContent.Where(x => x is IValueExpression).Any();
                    allOperatorsAreCommas = normalizedContent
						.Where(x => x is OperatorExpression)
						.Cast<OperatorExpression>()
						.All(x => x.Token.Value == Operator.Comma);
                    var allOtherAreVals = normalizedContent
						.Where(x => x is not OperatorExpression)
						.All(x => x is IValueExpression);
                    if (hasValExps && allOperatorsAreCommas && allOtherAreVals)
                    {
                        return new CollectionExpression(normalizedContent.Where(x => x is not OperatorExpression));
                    }

                    return new IndexerExpression(frame.Pop(), normalizedContent);
				default:
					throw new InvalidDataException("Unexpected consumable start");
			}
		}
	}
}
