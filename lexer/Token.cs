using System;

namespace lexer 
{
    public class Token
    {
        public enum Type { EOF }

        public Enum type;       // Type of token
        public String value;    // Literal value of token
        public Location start;  // Location just before the token
        public Location end;    // Location just after the token

        public Token(Location start, Location end, Enum type, String value)
        {
            this.start = start;
            this.end = end;
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            var typeStr = Enum.GetName(type.GetType(), type);
            return String.Format("[{0}:{1}]", typeStr, value);
        }
    }
}