using System;

namespace langsharp_core 
{
    public enum Token
    { 
        Keyword, Identifier, ParentStart, ParentEnd, 
        BraceStart, BraceEnd, Hash, String, Semicolon, 
        Integer, Float, EOF 
    };

    public class CLexer : lexer.Lexer
    {
        public CLexer(string fileName) : base(fileName) {}

        protected override Enum LexOne(char first)
        {
            // Lex keywords and identifiers
            if (Char.IsLetter(first))
            {
                while (Char.IsLetter(Peek())) Consume();
                if (IsKeyword()) return Token.Keyword;
                return Token.Identifier;
            }
            // Lex integers and floats
            if(Char.IsDigit(first)) 
            {
                while(Char.IsDigit(Peek())) Consume();
                if(Peek() == '.') {
                    Consume();
                    while(Char.IsDigit(Peek())) Consume();
                    return Token.Float;
                }
                return Token.Integer;
            }
            // Lex strings
            if(first == '"') 
            {
                while(!EOF && Peek() != '"') Consume();
                Consume();
                tokenValue = tokenValue.Substring(1, tokenValue.Length-2);
                return Token.String;
            }
            // Lex symbols
            switch(first)
            {
                case '{': return Token.BraceStart;
                case '}': return Token.BraceEnd;
                case '(': return Token.ParentStart;
                case ')': return Token.ParentEnd;
                case '#': return Token.Hash;
                case ';': return Token.Semicolon;                   
            }
            
            return null;
        }

        private bool IsKeyword()
        {
            switch(tokenValue)
            {
              case "include": return true;
              case "int":     return true;
              case "return":  return true;
            }
            return false;
        }
    }
}