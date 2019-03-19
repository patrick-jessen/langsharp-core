using System;
using System.IO;

namespace lexer
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

        public Lexer(string fileName)
        {
            this.fileName = fileName;
            source = File.ReadAllText(fileName);
        }

        public bool EOF => (index >= source.Length);

        public Token Lex()
        {
            while (true) {
                // Reset token
                tokenStart = GetLocation();
                tokenValue = "";
                skipToken = false;

                if (EOF) return MakeToken(Token.Type.EOF);

                var first = Consume();
                

                // Lex a single token
                var type = LexOne(first);
                if(type != null) return MakeToken(type);

                if(!skipToken)
                    throw new Exception(tokenStart, $"unexpected token '{tokenValue}'");
            }
        }

        protected char Peek()
        {
            if (EOF) return (char)0;
            return source[index];
        }
        protected char Consume()
        {
            var c = Peek();
            if (c == '\n')
            {
                lineNo++;
                lineStartIndex = index+1;
            }

            tokenValue += c;
            index++;
            return c;
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
    
        protected abstract Enum LexOne(char first);
    }
}