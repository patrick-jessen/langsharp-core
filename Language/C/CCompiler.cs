using System;
using Newtonsoft.Json;
using Language.C.Parsing;

namespace Language.C
{
    class CCompiler
    {
        public static void Compile(string file)
        {
            var p = new CParser(file);

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