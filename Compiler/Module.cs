using System.Collections.Generic;
using Compiler.Parsing;
using Compiler.Source;

class Module
{
    public string name;
    public List<SourceFile> files = new List<SourceFile>();

    public Module(string name)
    {
        this.name = name;
    }
}