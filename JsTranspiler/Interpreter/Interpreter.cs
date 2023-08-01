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
        public Scope Global = new();

        public Interpreter()
        {
            Global.CompositeObjects.Add(new IdentifierToken("console"), new Dictionary<IdentifierToken, ITokenExpression>()
            {
                [new IdentifierToken("log")] = new HookFunctionDefinitionExpression(
                    new DefinitionToken("function"),
                    new IdentifierToken("log"),
                    (string arg) => { Console.WriteLine(arg); },
                    new GroupExpression(new[] {
                        new SingleTokenExpression<IdentifierToken>(new IdentifierToken("arg"), SingleTokenExpressionType.Identifier)
                    }))
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
                            break;
                        case Operator.Access:
                            var arg1Token = (binOpExpr.Arg1 as SingleTokenExpression<IdentifierToken>).Token;
                            var arg2Token = ((binOpExpr.Arg2 as InvokationExpression).Action as SingleTokenExpression<IdentifierToken>).Token;
                            var globalScopeTarget = scope.CompositeObjects[arg1Token][arg2Token];
                            var methodArgument = Execute((binOpExpr.Arg2 as InvokationExpression).Expressions, scope) as SingleTokenExpression<NumberToken>;
                            (globalScopeTarget as HookFunctionDefinitionExpression).Hook(methodArgument.Token.Value.ToString());
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

            return result;
        }

        private ITokenExpressionContainer Containerize(ITokenExpression expression)
        {
            return new GroupExpression(new[] { expression });
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
            scope.TryGetValue(targetMethodIdentifier, out var userScopeMethodDefinition);
            var userScopeMethodDefinitionExpression = userScopeMethodDefinition as FunctionDefinitionExpression;
            var expectedArguments = userScopeMethodDefinitionExpression.Arguments.Expressions.Where(x => x is GroupExpression)
                .Cast<GroupExpression>()
                .Select(x => Execute(x, scope) as SingleTokenExpression<IdentifierToken>)
                .Select(x => x.Token)
                .ToArray();
            var methodScope = new Scope();
            for (int i = 0; i < expectedArguments.Count(); i++)
            {
                var argIdentifier = expectedArguments[i];
                var argValue = targetArgs[i];

                methodScope.Add(argIdentifier, argValue);
            }
            methodScope = methodScope.MergeWith(scope);
            return Execute(userScopeMethodDefinitionExpression.Value, methodScope);
        }
    }
}
