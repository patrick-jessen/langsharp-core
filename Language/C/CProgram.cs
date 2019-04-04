using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Language.C.Parsing;
using Compiler.Source;

namespace Language.C
{
    public class CProgram
    {
        public List<CFile> files = new List<CFile>();

        public CProgram(string[] sourceFiles)
        {
            foreach(var file in sourceFiles)
                this.files.Add(new CFile(this, new SourceFile(file)));
        }

        public void AddFile(string path)
        {
            foreach(var f in files) 
            {
                if(f.file.path == path) return;
            }
            this.files.Add(new CFile(this, new SourceFile(path)));
        }

        public void Compile()
        {
            for(var i = 0; i < files.Count; i++)
            {
                var f = files[i];

                Console.WriteLine($"####################### Parsing {f.file.path} #######################");
                f.Parse();
            }
        }
    }
}