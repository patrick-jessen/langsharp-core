using System;
using Compiler.Source;

namespace Compiler.Lexing
{
    public class TokenType {
        private string text;
        public TokenType(string text)
        {
            this.text = text;
        }
        public override string ToString() { return text; }
    };

    public class Token
    {
        public static TokenType EOF = new TokenType("end of file");

        public TokenType type;  // Type of token
        public String value;    // Literal value of token
        public SourcePos start; // Position just before the token
        public SourcePos end;   // Position just after the token

        public Token(SourcePos start, SourcePos end, TokenType type, String value)
        {
            this.start = start;
            this.end = end;
            this.type = type;
            this.value = value;
        }

        public bool Is(TokenType type = null, String value = null)
        {
            if(type != null && !this.type.Equals(type)) return false;
            if(value != null && this.value != value) return false;
            return true;
        }

        public override string ToString()
        {
            return String.Format("[{0}:{1}]", type, value);
        }
    }
}