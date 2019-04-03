using System;
using Newtonsoft.Json;
using Language.C.Parsing;
using Compiler.Source;

namespace Language.C
{
    class CCompiler
    {
        public static void Compile(string filePath)
        {
            var file = new SourceFile(filePath);

            var p = new CParser(file);
            var ast = p.Parse();

            if(file.errors.Count > 0)
            {
                foreach(var err in file.errors)
                    Console.WriteLine(file.ErrorToString(err));
            }
            else 
            {
                var json = JsonConvert.SerializeObject(ast, Formatting.Indented);
                Console.WriteLine(json);
            }
        }
    }
}