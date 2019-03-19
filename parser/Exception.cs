using System;
using lexer;

namespace parser 
{
    class Exception : System.Exception
    {
        public Location location;

        public Exception(Location location, string msg) : base(msg)
        {
            this.location = location;
        }

        public override string Message => String.Format("[ERROR] {0} at {1}", base.Message, location);
    }
}