using System;
using Compiler.Lexing;

namespace Compiler 
{
    public class CompilerError : Exception
    {
        public Location location;
        public string message;

        public CompilerError(Location location, string message)
        {
            this.location = location;
            this.message = message;
        }

        public override string Message => String.Format("[ERROR] {0} at {1}", message, location);
    }
}