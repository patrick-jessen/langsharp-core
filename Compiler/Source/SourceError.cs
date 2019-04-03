using System;
using Compiler.Lexing;

namespace Compiler.Source
{
    public class SourceError
    {
        public SourcePos pos;
        public string message;

        public SourceError(SourcePos pos, string message)
        {
            this.pos = pos;
            this.message = message;
        }
    }
}