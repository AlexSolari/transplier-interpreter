using System.Diagnostics;
using JsTranspiler.Interpreter;
using JsTranspiler.Parsing;
using JsTranspiler.Tokenizing;

var path = ".\\TestData\\9.js";//Console.ReadLine();
var tokenizer = new Tokenizer();

var data = File.ReadAllText(path);

var tokenizerStopwatch = Stopwatch.StartNew();
var tokens = tokenizer.Tokenize(data);
tokenizerStopwatch.Stop();
Console.WriteLine($"Tokenizing done. Time elaplsed: {tokenizerStopwatch.ElapsedMilliseconds}ms");

var parserStopwatch = Stopwatch.StartNew();
var parser = new Parser(tokens);
var expressions = parser.Parse();
parserStopwatch.Stop();
Console.WriteLine($"Parsing done. Time elaplsed: {parserStopwatch.ElapsedMilliseconds}ms");

var instructions = expressions.Reverse().ToList();

var interpreterStopwatch = Stopwatch.StartNew();
var interpreter = new Interpreter();
var result = interpreter.Execute(instructions, interpreter.Global);
interpreterStopwatch.Stop();
Console.WriteLine($"Executing done. Time elaplsed: {interpreterStopwatch.ElapsedMilliseconds}ms");

Console.ReadKey();