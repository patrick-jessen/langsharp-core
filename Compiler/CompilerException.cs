using System;
using Compiler.Lexing;

namespace Compiler 
{
    public class CompilerException : System.Exception
    {
        public Location location;

        public CompilerException(Location location, string msg) : base(msg)
        {
            this.location = location;
        }
        
        public override string Message => String.Format("[ERROR] {0} at {1}", base.Message, location);
    }
}