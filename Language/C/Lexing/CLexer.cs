using System;
using Compiler.Lexing;
using Compiler.Source;

namespace Language.C.Lexing
{
    public class CLexer : Lexer
    {
        public CLexer(SourceFile file) : base(file) {}

        protected override TokenType LexOne()
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
                if (IsKeyword()) return CTokens.Keyword;
                return CTokens.Identifier;
            }
            // Lex integers and floats
            if(Char.IsDigit(currChar)) 
            {
                while(Char.IsDigit(nextChar)) Consume();
                if(nextChar == '.') {
                    Consume();
                    while(Char.IsDigit(nextChar)) Consume();
                    return CTokens.Float;
                }
                return CTokens.Integer;
            }
            // Lex strings
            if(currChar == '"') 
            {
                while(!EOF && nextChar != '"') Consume();
                if(nextChar != '"') 
                    ReportError("expected '\"'");
                else 
                    Consume();
                tokenValue = tokenValue.Substring(1, tokenValue.Length-2);
                return CTokens.String;
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
                case '{': return CTokens.BraceStart;
                case '}': return CTokens.BraceEnd;
                case '(': return CTokens.ParentStart;
                case ')': return CTokens.ParentEnd;
                case '#': return CTokens.Hash;
                case ';': return CTokens.Semicolon;     
                case ',': return CTokens.Comma;              
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