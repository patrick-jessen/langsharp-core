using System;
using Compiler.Lexing;

namespace Language.C.Lexing
{
    public class CLexer : Lexer
    {
        public CLexer(string fileName) : base(fileName) {}

        protected override Enum LexOne()
        {
            // Ignore whitespace
            if (Char.IsWhiteSpace(currChar))
            {
                while(!EOF && Char.IsWhiteSpace(nextChar)) Consume();

                // Ignore white space
                skipToken = true;
                return null;
            }
            // Lex keywords and identifiers
            if (Char.IsLetter(currChar))
            {
                while (Char.IsLetter(nextChar)) Consume();
                if (IsKeyword()) return TokenType.Keyword;
                return TokenType.Identifier;
            }
            // Lex integers and floats
            if(Char.IsDigit(currChar)) 
            {
                while(Char.IsDigit(nextChar)) Consume();
                if(nextChar == '.') {
                    Consume();
                    while(Char.IsDigit(nextChar)) Consume();
                    return TokenType.Float;
                }
                return TokenType.Integer;
            }
            // Lex strings
            if(currChar == '"') 
            {
                while(!EOF && nextChar != '"') Consume();
                Consume();
                tokenValue = tokenValue.Substring(1, tokenValue.Length-2);
                return TokenType.String;
            }
            // Lex comments
            if(currChar == '/')
            {
                var second = Consume();
                if(second == '/')
                {
                    while(!EOF && nextChar != '\n') Consume();
                    skipToken = true;
                    return null;
                }
                if(second == '*')
                {
                    var v = Consume();
                    while(!EOF && (v != '*' || nextChar != '/')) v = Consume();
                    Consume();
                    skipToken = true;
                    return null;
                }
            }
            // Lex symbols
            switch(currChar)
            {
                case '{': return TokenType.BraceStart;
                case '}': return TokenType.BraceEnd;
                case '(': return TokenType.ParentStart;
                case ')': return TokenType.ParentEnd;
                case '#': return TokenType.Hash;
                case ';': return TokenType.Semicolon;     
                case ',': return TokenType.Comma;              
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
              case "extern":  return true;
            }
            return false;
        }
    }
}