using System;
using System.IO;
using Compiler.Source;

namespace Compiler.Lexing
{
    public abstract class Lexer
    {
        // Source code
        private SourceFile file;        // The file being lexed
        private string source;          // Contents of the file
        // Position tracking
        private int index;              // Current index into source
        private int lineNo;             // Current line number
        private int lineStartIndex;     // The index at which the current line starts
        // Error handling
        private bool isError;           // Whether we are currently lexing an invalid token
        private string errorValue;      // Value of the error
        private SourcePos errorStart;   // The start position of the error
        // Token handling
        private SourcePos tokenStart;   // The start position of the token currently being evaluated
        protected string tokenValue;    // Value of the token currently being evaluated
        protected bool skipToken;       // Whether to skip the current token
        protected char currChar;        // The current character
        protected char nextChar;        // The next character

        public Lexer(SourceFile file)
        {
            this.file = file;

            // Read entire file
            source = File.ReadAllText(file.path);

            // Make nextChar point to first char
            if(source.Length > 0)
                nextChar = source[0];
        }

        protected abstract TokenType LexOne();
        
        public bool EOF => (index >= source.Length);

        public Token Lex()
        {
            while (true) {
                // Reset token
                tokenStart = GetPosition();
                tokenValue = "";
                skipToken = false;

                // Handle EOF
                if (EOF) return MakeToken(Token.EOF);

                // Advance to next character
                Consume();

                // Lex a single token
                var type = LexOne();
                if(type != null) 
                    return MakeToken(type);
                if(skipToken)
                    continue;

                // Handle error
                if(!isError) 
                    errorStart = tokenStart;
                isError = true;
                errorValue += tokenValue;
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

        private SourcePos GetPosition()
        {
            return new SourcePos(lineNo + 1, index-lineStartIndex + 1);
        }

        private Token MakeToken(TokenType type)
        {
            if(isError)
            {
                // Report the previous error
                file.ReportError(new SourceError(errorStart, $"unexpected token {errorValue}"));
                isError = false;
                errorValue = "";
            }

            var end = GetPosition();
            return new Token(tokenStart, end, type, tokenValue);
        }

        protected void ReportError(string message)
        {
            file.ReportError(new SourceError(GetPosition(), message));
        }
    }
}