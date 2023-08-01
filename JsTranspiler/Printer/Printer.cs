using JsTranspiler.Parsing.Expressions;

public class Printer
{
    public static void Print(IEnumerable<ITokenExpression> tree){
        foreach(var exp in tree)
        {
            Console.WriteLine(exp.ToString());
        }
    }
}