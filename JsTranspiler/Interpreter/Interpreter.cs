﻿using JsTranspiler.Parsing.Expressions;
using JsTranspiler.Parsing.Expressions.Impl;
using JsTranspiler.Tokenizing.Tokens;
using JsTranspiler.Utils;
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
        private bool IsInsideTheConstructor = false;
        private IdentifierToken TempIdentifier = null;

        private readonly IdentifierToken module = new IdentifierToken("module");
		private readonly IdentifierToken exports = new IdentifierToken("exports");
		private readonly IdentifierToken console = new IdentifierToken("console");
		private readonly IdentifierToken log = new IdentifierToken("log");
        private readonly IdentifierToken error = new IdentifierToken("error");
        private readonly IdentifierToken constructor = new IdentifierToken("constructor");

		public Scope Global = new();

        public Interpreter()
        {
            Global[console] = new ObjectExpression();
            Global[console][log] = new HookFunctionDefinitionExpression(
                    new DefinitionToken("function"),
                    log,
                    (string arg) => { Console.WriteLine($"[LOG]: {arg}"); },
                    new GroupExpression(new[] {
                        new IdentifierExpression(new IdentifierToken("arg"))
                    })); 
            Global[console][error] = new HookFunctionDefinitionExpression(
                    new DefinitionToken("function"),
                    error,
                    (string arg) => { Console.WriteLine($"[ERROR]: {arg}"); },
                    new GroupExpression(new[] {
                        new IdentifierExpression(new IdentifierToken("arg"))
                    }));

            Global[module] = new ObjectExpression();
            Global[module][exports] = new NullExpression();
		}

        public ITokenExpression Execute(ITokenExpression tokenExpression, Scope scope)
        {
            return Execute(new[] { tokenExpression }, scope);
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
                        scope[definition.Identifier] = new ObjectExpression();
                        foreach (var method in classMethods)
                        {
                            scope[definition.Identifier][method.Identifier] = method;
                        }
                    }
                    else
                    {
                        var funcDef = definition.Value is ObjectExpression
                            ? definition.Value
                            : definition.Value is InvokationExpression invExp
                                ? ProcessInvokation(invExp, scope)
                                : Execute(definition.Value, scope);
                        scope[definition.Identifier] = funcDef as IValueExpression;
                        result = funcDef;
                    }
                } 
                else if (instruction is GroupExpression group)
                {
                    result = Execute(group, scope).Unwrap();
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
                            return Execute(unKeywordExp.Arg1, scope);
						case Keyword.New:
							result = Execute(unKeywordExp.Arg1, scope);
							break;
						default:
                            break;
                    }
                }
				else if (instruction is UnaryExpression<OperatorToken> unOpExp)
				{
					switch (unOpExp.Operator.Value)
					{
						case Operator.Not:
							result = Execute(unOpExp.Arg1, scope);
                            if (result is PrimitiveExpression<BooleanToken> boolRes)
                            {
                                result = new PrimitiveExpression<BooleanToken>(new BooleanToken(!boolRes.Token.Value));
                            }
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
                            var arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
                            var arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

                            result = new PrimitiveExpression<NumberToken>(new NumberToken(arg1.Token.Value + arg2.Token.Value));
                            break;
                        case Operator.Minus:
                            arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

                            result = new PrimitiveExpression<NumberToken>(new NumberToken(arg1.Token.Value - arg2.Token.Value));
                            break;
                        case Operator.Multiplication:
                            arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

                            result = new PrimitiveExpression<NumberToken>(new NumberToken(arg1.Token.Value * arg2.Token.Value));
                            break;
                        case Operator.Division:
                            arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

                            result = new PrimitiveExpression<NumberToken>(new NumberToken(arg1.Token.Value / arg2.Token.Value));
                            break;
                        case Operator.WholeDivision:
                            arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
                            arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

                            result = new PrimitiveExpression<NumberToken>(new NumberToken(arg1.Token.Value % arg2.Token.Value));
                            break;
                        case Operator.Equals:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value == arg2.Token.Value));
							break;
                        case Operator.NotEquals:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value != arg2.Token.Value));
							break;
                        case Operator.Assign:
							ProcessAssignments(binOpExpr, scope);
							break;
                        case Operator.Access:
                            result = ProcessAccess(binOpExpr, scope);
                            break;
                        case Operator.More:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value > arg2.Token.Value));
							break;
                        case Operator.MoreEquals:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value >= arg2.Token.Value));
							break;
                        case Operator.Less:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value < arg2.Token.Value));
							break;
                        case Operator.LessEquals:
							arg1 = Execute(binOpExpr.Arg1, scope) as SingleTokenExpression<NumberToken>;
							arg2 = Execute(binOpExpr.Arg2, scope) as SingleTokenExpression<NumberToken>;

							result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1.Token.Value <= arg2.Token.Value));
							break;
                        case Operator.Comma:
                            break;
                        case Operator.And:
                            var arg1Bool = ConditionIsTrue(Execute(binOpExpr.Arg1, scope));
                            var arg2Bool = ConditionIsTrue(Execute(binOpExpr.Arg2, scope));

                            result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1Bool && arg2Bool));
                            break;
                        case Operator.Or:
                            arg1Bool = ConditionIsTrue(Execute(binOpExpr.Arg1, scope));
                            arg2Bool = ConditionIsTrue(Execute(binOpExpr.Arg2, scope));

                            result = new PrimitiveExpression<BooleanToken>(new BooleanToken(arg1Bool || arg2Bool));
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
                else if (instruction is BinaryExpression<KeywordToken> binKeywExpr)
                {
                    switch (binKeywExpr.Operator.Value)
                    {
                        case Keyword.Else:
                            var ifExpr = binKeywExpr.Arg1.Unwrap() as BinaryExpression<KeywordToken>;
                            var condition = Execute(ifExpr.Arg1, scope);
                            if (ConditionIsTrue(condition))
                            {
                                result = Execute(ifExpr.Arg2, scope);
                            }
                            else
                            {
								result = Execute(binKeywExpr.Arg2, scope);
							}
                            break;
                        case Keyword.If:
							condition = Execute(binKeywExpr.Arg1, scope);
							if (ConditionIsTrue(condition))
							{
								result = Execute(binKeywExpr.Arg2, scope);
							}
							break;
                        default:
                            break;
                    }
                }
                else if (instruction is IdentifierExpression identifierSteExp)
                {
                    result = scope.ContainsKey(identifierSteExp.Token)
                        ? scope[identifierSteExp.Token]
                        : identifierSteExp;
				}
				else if (instruction is IValueExpression valueExp)
				{
					result = valueExp;
				}
				else if (instruction is BlockExpression blockExpression)
                {
                    result = Execute(blockExpression, scope);
                }
            }

            return result ?? new ObjectExpression();
        }

		private bool ConditionIsTrue(ITokenExpression condition)
		{
			if (condition is PrimitiveExpression<BooleanToken> boolExp)
            {
                return boolExp.Token.Value;
            }
            else if (condition is not NullExpression)
            {
                return true;
            }

            return false;
		}

		private ITokenExpression ProcessAccess(BinaryExpression<OperatorToken> binOpExpr, Scope scope)
		{

			var arg1Token = ((binOpExpr.Arg1.Unwrap()) as IdentifierExpression).Token;
            var arg2 = binOpExpr.Arg2.Unwrap();
            if (arg2 is SingleTokenExpression<IdentifierToken> steArg2)
            {
			    return scope[arg1Token][steArg2.Token];

            }
            else if (arg2 is InvokationExpression invExp) 
            {
				var arg2Token = ((binOpExpr.Arg2 as InvokationExpression).Action as IdentifierExpression).Token;
				var methodDefinitionExpression = scope[arg1Token][arg2Token] as FunctionDefinitionExpression;
				var methodScope = new Scope();
				methodScope.Add(arg2Token, methodDefinitionExpression);
				methodScope = methodScope.MergeWith(scope);
				return ProcessInvokation(invExp, methodScope);
			}
            else if (arg2 is BinaryExpression<OperatorToken> binOpExp
                && binOpExp.Operator.Value == Operator.Access)
            {
                var test = new BinaryExpression<OperatorToken>(binOpExpr.Arg1, binOpExp.Arg1, new OperatorToken(Operator.Access));
                var foo = ProcessAccess(test, scope);

                if (binOpExp.Arg2 is InvokationExpression classInvExp)
                {
                    var instance = foo as ObjectExpression;
                    var classMethod = scope[instance.InstanceOf][(classInvExp.Action as IdentifierExpression).Token] as FunctionDefinitionExpression;
					var methodScope = new Scope();
					methodScope.Add((classInvExp.Action as IdentifierExpression).Token, classMethod);
                    foreach (var item in instance.Values)
                    {
                        var identifier = (item.Key as IdentifierExpression).Token;

						methodScope[identifier] = item.Value;
                    }
					methodScope = methodScope.MergeWith(scope);
					return ProcessInvokation(classInvExp, methodScope);
                }

                var bar = new BinaryExpression<OperatorToken>(foo, binOpExp.Arg2, new OperatorToken(Operator.Access));

                return bar;
            }
            else if (arg2 is IndexerExpression indexAccessExp)
            {
                return AccessByIndex(indexAccessExp, scope);
            }

            throw new NotImplementedException();

            ITokenExpression AccessByIndex(IndexerExpression indexAccessExp, Scope scope)
            {
                if (indexAccessExp.Container is IdentifierExpression identifierExpression)
                {
                    var arg2 = Execute(indexAccessExp.Expressions, scope) as IValueExpression;
                    var obj = scope[identifierExpression.Token] as ObjectExpression;
                    if (obj.Values.ContainsKey(arg2))
                    {
                        return obj[arg2];
                    }
                    return new NullExpression();
                }
                else if (indexAccessExp.Container is BinaryExpression<OperatorToken> bExp)
                {
                    return ProcessAccess(bExp, scope);
                }
                else if (indexAccessExp.Container is IndexerExpression innerIndexerExp)
                {
                    return AccessByIndex(innerIndexerExp, scope);
                }

                throw new NotImplementedException();
            }
		}

		public ITokenExpression ProcessInvokation(InvokationExpression invokationExpression, Scope scope)
        {
            var targetMethodIdentifier = (invokationExpression.Action as SingleTokenExpression<IdentifierToken>).Token;
            var targetArgs = invokationExpression.Expressions
                .Where(x => x is not SingleTokenExpression<OperatorToken> op)
                .Select(x => Execute(x, scope) as ISingleTokenExpression)
                .Select(x => x.Token)
                .Select(x => {
                        if (x is IdentifierToken iX && scope.ContainsKey(iX))
                        {
                            return scope[iX];
                        }
                        else if (x is NumberToken nX)
                        {
                            return new PrimitiveExpression<NumberToken>(nX);
                        }
                        else if (x is StringToken sX)
                        {
                            return new PrimitiveExpression<StringToken>(sX);
                        }
					    else if (x is BooleanToken bX)
					    {
						    return new PrimitiveExpression<BooleanToken>(bX);
					    }

					    throw new InvalidOperationException($"Identifier {x} is not defined");
                })
                .ToArray();
            scope.TryGetValue(targetMethodIdentifier, out var scopeMethodDefinition);

            if (scopeMethodDefinition is not FunctionDefinitionExpression)
            {
                if (scope.ContainsKey(targetMethodIdentifier))
                {
                    scopeMethodDefinition = scope[targetMethodIdentifier][constructor];
                    IsInsideTheConstructor = true;
                    TempIdentifier = new IdentifierToken($"{targetMethodIdentifier}_constructor_{Guid.NewGuid()}");
                }
                else
                {
                    throw new InvalidOperationException($"{targetMethodIdentifier} does not exist");
                }
            }

            var scopeMethodDefinitionExpression = scopeMethodDefinition as FunctionDefinitionExpression;
            var expectedArguments = scopeMethodDefinitionExpression.Arguments.Expressions
                .Select(x => x.Unwrap())
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
                var argExp = targetArgs[0].Unwrap();
                var arg = string.Empty;

                if (argExp is PrimitiveExpression<StringToken> stringSte)
                {
                    arg = stringSte.Token.Value;
                }
				else if(argExp is PrimitiveExpression<NumberToken> numberSte)
				{
                    arg = numberSte.Token.Value.ToString();
                }
                else if (argExp is PrimitiveExpression<BooleanToken> boolSte)
                {
                    arg = boolSte.Token.Value.ToString();
                }

				hookFunctionExp.Hook.Invoke(arg);

                return new NullExpression();
			}
            else
            {
                var result = Execute(scopeMethodDefinitionExpression.Value, methodScope);
                if (IsInsideTheConstructor)
                {
                    IsInsideTheConstructor = false;
                    result = methodScope[TempIdentifier];
                    (result as ObjectExpression).InstanceOf = targetMethodIdentifier;
					TempIdentifier = null;
                }

				return result;
            }

        }


		private void ProcessAssignments(BinaryExpression<OperatorToken> binOpExpr, Scope scope)
		{
			IdentifierToken arg1Identifier;
            var arg1 = binOpExpr.Arg1.Unwrap();
			var arg2 = binOpExpr.Arg2.Unwrap();

			if (arg1 is IdentifierExpression identifierSte)
            {
                arg1Identifier = identifierSte.Token;
            }
            else if (arg1 is BinaryExpression<OperatorToken> bOpExp)
            {
				var arg1Parent = ((bOpExp.Arg1.Unwrap()) as IdentifierExpression).Token;
				var arg1Child = ((bOpExp.Arg2.Unwrap()) as IdentifierExpression).Token;

                if (IsInsideTheConstructor)
                {
                    if (!scope.ContainsKey(TempIdentifier))
                    {
                        scope[TempIdentifier] = new ObjectExpression();
                    }
                    var key = ((bOpExp.Arg2.Unwrap()) as IValueExpression);

					(scope[TempIdentifier] as ObjectExpression)[key] = Execute(binOpExpr.Arg2, scope) as IValueExpression;
				}
                else
                {
    			    scope[arg1Parent][arg1Child] = Execute(binOpExpr.Arg2, scope) as IValueExpression;
                }

            }
		}

	}
}
