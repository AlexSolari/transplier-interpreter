using JsTranspiler.Parsing.Expressions;
using JsTranspiler.Parsing.Expressions.Impl;
using JsTranspiler.Tokenizing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using BlockExpression = JsTranspiler.Parsing.Expressions.Impl.BlockExpression;

namespace JsTranspiler.Interpreter
{
    public class Interpreter
    {
        private readonly IdentifierToken module = new IdentifierToken("module");
		private readonly IdentifierToken exports = new IdentifierToken("exports");
		private readonly IdentifierToken console = new IdentifierToken("console");
		private readonly IdentifierToken log = new IdentifierToken("log");
		private readonly IdentifierToken constructor = new IdentifierToken("constructor");

		public Scope Global = new();

        public Interpreter()
        {
            Global.CompositeObjects.Add(console, new Dictionary<IdentifierToken, ITokenExpression>()
            {
                [log] = new HookFunctionDefinitionExpression(
                    new DefinitionToken("function"),
					log,
                    (string arg) => { Console.WriteLine(arg); },
                    new GroupExpression(new[] {
                        new SingleTokenExpression<IdentifierToken>(new IdentifierToken("arg"), SingleTokenExpressionType.Identifier)
                    }))
            });

            Global.CompositeObjects.Add(module, new Dictionary<IdentifierToken, ITokenExpression>()
            {
                [exports] = new SingleTokenExpression<NumberToken>(new NumberToken(0), SingleTokenExpressionType.Constant)
            });
		}

        public ITokenExpression Execute(ITokenExpressionContainer expressionContainer, Scope scope)
        {
            return Execute(expressionContainer.Expressions, scope);
        }

        public ITokenExpression Execute(IEnumerable<ITokenExpression> instructions, Scope scope)
        {
            ITokenExpression result = null;
            foreach (var instruction in instructions)
            {
                if (instruction is DefinitionExpression definition)
                {
                    if (definition is FunctionDefinitionExpression functionDefinition)
                    {
                        scope.Add(definition.Identifier, functionDefinition);
                        result = functionDefinition;
                    }
                    else if (definition.Definition.Value.Equals("class"))
                    {
                        var classMethods = definition.Value.Expressions.Cast<DefinitionExpression>();
                        scope.CompositeObjects[definition.Identifier] = new();
                        foreach (var method in classMethods)
                        {
                            scope.CompositeObjects[definition.Identifier][method.Identifier] = method;
                        }
                        ;
                    }
                    else
                    {
                        var funcDef = Execute(definition.Value, scope);
                        scope.Add(definition.Identifier, funcDef);
                        result = funcDef;
                    }
                } 
                else if (instruction is GroupExpression group)
                {
                    var val = Execute(group, scope);

                    if (val is GroupExpression ge
                        && ge.Expressions.Count() == 1)
                    {
                        result = ge.Expressions.First();
                    }
                    else
                    {
                        result = val;
                    }
                }
                else if (instruction is InvokationExpression invokExp)
                {
                    result = ProcessInvokation(invokExp, scope);
                }
                else if (instruction is UnaryExpression<KeywordToken> unKeywordExp)
                {
                    switch (unKeywordExp.Operator.Value)
                    {
                        case Keyword.Return:
                        case Keyword.New:
                            result = Execute(unKeywordExp.Arg1 as ITokenExpressionContainer, scope);
                            break;
                        default:
                            break;
                    }
                }
                else if (instruction is BinaryExpression<OperatorToken> binOpExpr)
                {
                    switch (binOpExpr.Operator.Value)
                    {
                        case Operator.Plus:
                            var arg1 = Execute(Containerize(binOpExpr.Arg1), scope) as SingleTokenExpression<NumberToken>;
                            var arg2 = Execute(Containerize(binOpExpr.Arg2), scope) as SingleTokenExpression<NumberToken>;

                            result = new SingleTokenExpression<NumberToken>(
                                new NumberToken(arg1.Token.Value + arg2.Token.Value),
                                SingleTokenExpressionType.Constant);
                            break;
                        case Operator.Minus:
                            arg1 = Execute(Containerize(binOpExpr.Arg1), scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(Containerize(binOpExpr.Arg2), scope) as SingleTokenExpression<NumberToken>;

                            result = new SingleTokenExpression<NumberToken>(
                                new NumberToken(arg1.Token.Value - arg2.Token.Value),
                                SingleTokenExpressionType.Constant);
                            break;
                        case Operator.Multiplication:
                            arg1 = Execute(Containerize(binOpExpr.Arg1), scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(Containerize(binOpExpr.Arg2), scope) as SingleTokenExpression<NumberToken>;

                            result = new SingleTokenExpression<NumberToken>(
                                new NumberToken(arg1.Token.Value * arg2.Token.Value),
                                SingleTokenExpressionType.Constant);
                            break;
                        case Operator.Division:
                            arg1 = Execute(Containerize(binOpExpr.Arg1), scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(Containerize(binOpExpr.Arg2), scope) as SingleTokenExpression<NumberToken>;

                            result = new SingleTokenExpression<NumberToken>(
                                new NumberToken(arg1.Token.Value / arg2.Token.Value),
                                SingleTokenExpressionType.Constant);
                            break;
                        case Operator.WholeDivision:
                            arg1 = Execute(Containerize(binOpExpr.Arg1), scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(Containerize(binOpExpr.Arg2), scope) as SingleTokenExpression<NumberToken>;

                            result = new SingleTokenExpression<NumberToken>(
                                new NumberToken(arg1.Token.Value % arg2.Token.Value),
                                SingleTokenExpressionType.Constant);
                            break;
                        case Operator.Equals:
                            break;
                        case Operator.NotEquals:
                            break;
                        case Operator.Assign:
							ProcessAssignments(binOpExpr, scope);
							break;
                        case Operator.Access:
                            result = ProcessAccess(binOpExpr, scope);
                            break;
                        case Operator.More:
                            break;
                        case Operator.MoreEquals:
                            break;
                        case Operator.Less:
                            break;
                        case Operator.LessEquals:
                            break;
                        case Operator.Comma:
                            break;
                        case Operator.And:
                            break;
                        case Operator.Or:
                            break;
                        case Operator.Increment:
                        case Operator.Decrement:
                        case Operator.Not:
                        case Operator.ArrowFunc:
                        case Operator.Ternary:
                        case Operator.JsonAssign:
                        case Operator.Unknown:
                        default:
                            throw new InvalidOperationException($"{binOpExpr.Operator.Value} cannot be used in binary expression {binOpExpr}");
                    }
                }
                else if (instruction is SingleTokenExpression<NumberToken> numberSteExp)
                {
                    result = numberSteExp;
                }
                else if (instruction is SingleTokenExpression<IdentifierToken> identifierSteExp)
                {
                    result = scope.ContainsKey(identifierSteExp.Token)
                        ? scope[identifierSteExp.Token]
                        : identifierSteExp;
                }
                else if (instruction is BlockExpression blockExpression)
                {
                    result = Execute(blockExpression, scope);
                }
            }

            return result ?? scope.CompositeObjects[module][exports];
        }

		private ITokenExpressionContainer Containerize(ITokenExpression expression)
        {
            return new GroupExpression(new[] { expression });
		}


		private ITokenExpression Unwrap(ITokenExpression expression)
		{
            if (expression is GroupExpression gExp)
            {
                if (gExp.Expressions.Count() == 1)
                    return Unwrap(gExp.Expressions.First());
            }

            return expression;
		}

		private ITokenExpression ProcessAccess(BinaryExpression<OperatorToken> binOpExpr, Scope scope)
		{

			var arg1Token = (Unwrap(binOpExpr.Arg1) as SingleTokenExpression<IdentifierToken>).Token;
            var arg2 = Unwrap(binOpExpr.Arg2);
            if (arg2 is SingleTokenExpression<IdentifierToken> steArg2)
            {
			    return scope.CompositeObjects[arg1Token][steArg2.Token];

            }
            else if (arg2 is InvokationExpression invExp) 
            {
				var arg2Token = ((binOpExpr.Arg2 as InvokationExpression).Action as SingleTokenExpression<IdentifierToken>).Token;
				var methodDefinitionExpression = scope.CompositeObjects[arg1Token][arg2Token] as FunctionDefinitionExpression;
				var methodScope = new Scope();
				methodScope.Add(arg2Token, methodDefinitionExpression);
				methodScope = methodScope.MergeWith(scope);
				return ProcessInvokation(invExp, methodScope);
			}

            throw new NotImplementedException();
		}

		public ITokenExpression ProcessInvokation(InvokationExpression invokationExpression, Scope scope)
        {
            var targetMethodIdentifier = (invokationExpression.Action as SingleTokenExpression<IdentifierToken>).Token;
            var targetArgs = invokationExpression.Expressions
                .Where(x => x is not SingleTokenExpression<OperatorToken> op)
                .Select(x => Execute(Containerize(x), scope) as ISingleTokenExpression)
                .Select(x => x.Token)
                .Select(x => {
                        if (x is IdentifierToken iX && scope.ContainsKey(iX))
                        {
                            return scope[iX];
                        }
                        else if (x is NumberToken nX)
                        {
                            return new GroupExpression(new[] { new SingleTokenExpression<NumberToken>(nX, SingleTokenExpressionType.Constant) });
                        }
                        else if (x is Token<string> sX)
                        {
                            return new GroupExpression(new[] { new SingleTokenExpression<Token<string>>(sX, SingleTokenExpressionType.Constant) });
                        }

                        throw new InvalidOperationException($"Identifier {x} is not defined");
                })
                .ToArray();
            scope.TryGetValue(targetMethodIdentifier, out var scopeMethodDefinition);

            if (scopeMethodDefinition == null)
            {
                if (scope.CompositeObjects.ContainsKey(targetMethodIdentifier))
                {
                    scopeMethodDefinition = scope.CompositeObjects[targetMethodIdentifier][constructor];
                }
                else
                {
                    throw new InvalidOperationException($"{targetMethodIdentifier} does not exist");
                }
            }

            var scopeMethodDefinitionExpression = scopeMethodDefinition as FunctionDefinitionExpression;
            var expectedArguments = scopeMethodDefinitionExpression.Arguments.Expressions
                .Select(x => Unwrap(x))
                .Select(x => (x as SingleTokenExpression<IdentifierToken>).Token)
                .ToArray();
            var methodScope = new Scope();
            for (int i = 0; i < expectedArguments.Count(); i++)
            {
                var argIdentifier = expectedArguments[i];
                var argValue = targetArgs[i];

                methodScope.Add(argIdentifier, argValue);
            }
            methodScope = methodScope.MergeWith(scope);

			if (scopeMethodDefinitionExpression is HookFunctionDefinitionExpression hookFunctionExp)
            {
                var argExp = Unwrap(targetArgs[0]);
                var arg = string.Empty;

                if (argExp is SingleTokenExpression<Token<string>> stringSte)
                {
                    arg = stringSte.Token.Value;
                }
				else if(argExp is SingleTokenExpression<NumberToken> numberSte)

				{
                    arg = numberSte.Token.Value.ToString();
                }

				hookFunctionExp.Hook.Invoke(arg);

                return new SingleTokenExpression<KeywordToken>(new KeywordToken(Keyword.Null), SingleTokenExpressionType.Constant);
			}
            else
            {
				return Execute(scopeMethodDefinitionExpression.Value, methodScope);
            }

        }


		private void ProcessAssignments(BinaryExpression<OperatorToken> binOpExpr, Scope scope)
		{
			IdentifierToken arg1Identifier;
            var arg1 = Unwrap(binOpExpr.Arg1);
			var arg2 = Unwrap(binOpExpr.Arg2);

			if (arg1 is SingleTokenExpression<IdentifierToken> identifierSte)
            {
                arg1Identifier = identifierSte.Token;
            }
            else if (arg1 is BinaryExpression<OperatorToken> bOpExp)
            {
				var arg1Parent = (Unwrap(bOpExp.Arg1) as SingleTokenExpression<IdentifierToken>).Token;
				var arg1Child = (Unwrap(bOpExp.Arg2) as SingleTokenExpression<IdentifierToken>).Token;

                var wrappedArg = binOpExpr.Arg2 is ITokenExpressionContainer wArg2
					? wArg2
					: Containerize(binOpExpr.Arg2);

				scope.CompositeObjects[arg1Parent][arg1Child] = Execute(wrappedArg, scope);
            }
		}

	}
}
