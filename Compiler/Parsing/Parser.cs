using System;
using System.Collections.Generic;
using Compiler.Lexing;

namespace Compiler.Parsing
{
    public abstract class Parser
    {
        private Lexer lexer;
        protected Token currToken;
        protected Token nextToken;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;

            // Make nextToken point to first token, and currToken to the "0-th" token
            nextToken = lexer.Lex();
            currToken = new Token(nextToken.start, nextToken.start, null, null);
        }

        protected abstract ASTNode ParseOne();

        public ASTNode Parse() 
        {
            ASTNode ast = ParseOne();
            Consume(Token.Type.EOF); // Make sure we are at EOF
            return ast;
        }

        protected Token Consume(Enum type = null, String value = null)
        {
            // Assert type and value
            if(!nextToken.Is(type, value))
                throw new CompilerException(currToken.end, String.Format("expected '{0}'", value==null?(object)type:value));

            // Advance tokens
            currToken = nextToken;
            nextToken = lexer.Lex();
            return currToken;
        }
    }
}