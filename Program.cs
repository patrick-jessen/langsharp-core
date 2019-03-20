using System;
using Newtonsoft.Json;
using Language.C;

namespace langsharp_core
{
    class Program
    {
        static void Main(string[] args)
        {
            CCompiler.Compile("./prog/prog.c");
        }
    }
}
