using System;

namespace langsharp_core
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parser p = new Parser("./prog/prog.c");
            // var ast = p.Parse();
            // Console.WriteLine(ast.ToString());
            var l = new CLexer("./prog/prog.c");
            while(!l.EOF) {
                Console.WriteLine(l.Lex());
            }
             Console.WriteLine(l.Lex());
        }
    }
}
