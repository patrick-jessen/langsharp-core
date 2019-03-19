using System;
using Newtonsoft.Json;

namespace langsharp_core
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new CParser("./prog/prog.c");

            try 
            {
                var ast = p.Parse();
                var json = JsonConvert.SerializeObject(ast, Formatting.Indented);
                Console.WriteLine(json);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
