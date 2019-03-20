using System;
using System.IO;

namespace Compiler.Lexing
{
    public abstract class Lexer
    {
        private string fileName;    // File which is being lexed
        private string source;      // Contents of the file
        private int index;          // Current index into source
        private int lineNo = 1;     // Current line number
        private int lineStartIndex; // The index at which the current line starts
        private Location tokenStart;// The start location of the token currently being evaluated

        protected string tokenValue;// Value of the token currently being evaluated
        protected bool skipToken;   // Whether to skip the current token
        protected char currChar;    // The current character
        protected char nextChar;    // The next character

        public Lexer(string fileName)
        {
            this.fileName = fileName;

            // Read entire file
            source = File.ReadAllText(fileName);

            // Make nextChar point to first char
            if(source.Length > 0)
                nextChar = source[0];
        }

        protected abstract Enum LexOne();
        
        public bool EOF => (index >= source.Length);

        public Token Lex()
        {
            while (true) {
                // Reset token
                tokenStart = GetLocation();
                tokenValue = "";
                skipToken = false;

                // Handle EOF
                if (EOF) return MakeToken(Token.Type.EOF);

                // Advance to next character
                Consume();

                // Lex a single token
                var type = LexOne();
                if(type != null) return MakeToken(type);

                if(!skipToken)
                    throw new CompilerException(tokenStart, $"unexpected token '{tokenValue}'");
            }
        }

        protected char Consume()
        {
            index++;

            // Update current and next char
            currChar = nextChar;
            if(EOF) nextChar = (char)0;
            else nextChar = source[index];
            
            // Keep track of lines
            if (currChar == '\n')
            {
                lineNo++;
                lineStartIndex = index;
            }
            
            // Add char to the value of token
            if(currChar != 0)
                tokenValue += currChar;

            return currChar;
        }

        private Location GetLocation()
        {
            return new Location(fileName, lineNo, index-lineStartIndex + 1);
        }

        private Token MakeToken(Enum type)
        {
            var end = GetLocation();
            end.columnNo += 1;
            return new Token(tokenStart, end, type, tokenValue);
        }
    }
}