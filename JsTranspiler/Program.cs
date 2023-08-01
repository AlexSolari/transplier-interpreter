using System.Diagnostics;
using JsTranspiler.Parsing;
using JsTranspiler.Tokenizing;

var path = "D:\\repos\\Transpiler\\test\\1.js";//Console.ReadLine();
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

var foo = expressions.Reverse().ToList();
Printer.Print(foo);
Console.ReadKey();
