using System;
using Newtonsoft.Json;
using Language.C;

namespace langsharp_core
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new CProgram(new string[]{
                "./prog/prog.c",
                "./prog/other.c"
            });
            prog.Compile();
        }
    }
}
