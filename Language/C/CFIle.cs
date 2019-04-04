using System;
using System.Collections.Generic;
using Compiler.Source;
using Language.C.Parsing;
using Newtonsoft.Json;

namespace Language.C
{
    public class CFile
    {
        public CProgram program;
        public SourceFile file;

        public List<ASTDeclaration> exports = null; // todo
        public List<ASTDeclaration> expects = null; // todo
        public List<string> includeFiles = new List<string>();

        public CFile(CProgram program, SourceFile file)
        {
            this.program = program;
            this.file = file;
        }

        public void AddInclude(string include)
        {
            includeFiles.Add(include);
            program.AddFile(include);
        }

        public void Parse()
        {
            var p = new CParser(this);
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
