using System;
using System.Collections.Generic;
using Compiler.Lexing;
using Compiler.Source;

namespace Compiler.Parsing
{
    public abstract class Parser
    {
        private SourceFile file;
        private Lexer lexer;
        protected Token currToken;
        protected Token nextToken;
        private bool isError;

        public Parser(SourceFile file, Lexer lexer)
        {
            this.file = file;
            this.lexer = lexer;

            // Make nextToken point to first token, and currToken to the "0-th" token
            nextToken = lexer.Lex();
            currToken = new Token(nextToken.start, nextToken.start, null, null);
        }

        protected abstract ASTNode ParseOne();

        public ASTNode Parse() 
        {
            ASTNode ast = ParseOne();
            Consume(Token.EOF); // Make sure we are at EOF
            return ast;
        }

        protected Token Consume(TokenType type = null, String value = null)
        {
            // Assert type and value
            if(!nextToken.Is(type, value)) 
            {
                var expect = type.ToString();
                if(value != null)
                    expect = $"\"{value}\"";

                ReportError(currToken.end, String.Format("expected {0}", expect));
                throw new Exception();
            }
            isError = false;

            // Advance tokens
            currToken = nextToken;
            nextToken = lexer.Lex();
            return currToken;
        }

        protected void ReportError(SourcePos pos, string message) 
        {
            if(!isError)
                file.ReportError(new SourceError(pos, message));
            isError = true;
        }

        protected void ConsumeAndReportError(SourcePos pos, string message)
        {
            if(isError) 
            {
                Consume();
                isError = true;            
            }
            ReportError(pos, message);
        }
    }
}