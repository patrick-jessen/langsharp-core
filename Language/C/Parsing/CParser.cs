using System;
using Compiler;
using Compiler.Lexing;
using Compiler.Parsing;
using Language.C.Lexing;

namespace Language.C.Parsing
{
    public class CParser : Parser 
    {
        public CParser(string fileName) : base(new CLexer(fileName)) {}

        protected override ASTNode ParseOne()
        {
            var ast = new ASTFile();

            while(nextToken.Is(TokenType.Hash))
                ast.includes.Add(ParseInclude());

            while(!nextToken.Is(Token.Type.EOF))
                ast.declarations.Add(ParseDeclaration());

            return ast;
        }

        private ASTInclude ParseInclude() 
        {
            Consume(TokenType.Hash);
            Consume(TokenType.Keyword, "include");
            var fileTok = Consume(TokenType.String);
            return new ASTInclude() { file = fileTok };
        }

        private ASTDeclaration ParseDeclaration()
        {
            if(nextToken.Is(TokenType.Keyword, "extern"))
                return ParseExtern();
            if(nextToken.Is(TokenType.Keyword))
                return ParseFunction();

            throw new CompilerError(nextToken.start, "expected declaration");
        }

        private ASTExtern ParseExtern() 
        {
            var ast = new ASTExtern();

            Consume(TokenType.Keyword, "extern");
            ast.declaration = ParseFunctionSignature();
            Consume(TokenType.Semicolon);
            return ast;
        }

        private ASTFunction ParseFunctionSignature()
        {
            var retTok = Consume(TokenType.Keyword);
            var nameTok = Consume(TokenType.Identifier);
            Consume(TokenType.ParentStart);
            Consume(TokenType.ParentEnd);

            return new ASTFunction() 
            {
                name = nameTok,
                returns = retTok
            };
        }

        private ASTFunction ParseFunction() 
        {
            var ast = ParseFunctionSignature();
            var bodyLoc = nextToken.end;

            try {
                Consume(TokenType.BraceStart);
                while(!nextToken.Is(TokenType.BraceEnd) && !nextToken.Is(Token.Type.EOF))
                {
                    ast.statements.Add(ParseStatement());
                    Consume(TokenType.Semicolon);
                }
                Consume(TokenType.BraceEnd);
            }
            catch(CompilerError e) {
                throw BestError(e, new CompilerError(bodyLoc, "expected function body"));
            }

            return ast;
        }

        ASTStatement ParseStatement() 
        {
            var stmtLoc = currToken.end;

            while(!nextToken.Is(Token.Type.EOF)) {
            try {
                if(nextToken.Is(TokenType.Keyword, "return"))
                {
                    Consume();
                    return new ASTReturn()
                    {
                        value = ParseExpression()
                    };
                }
                else if(nextToken.Is(TokenType.Identifier))
                    return ParseFunctionCall();
                else
                    Consume();
            }
            catch(CompilerError e) {
                var err = BestError(e, new CompilerError(stmtLoc, "expected statement"));
                Console.WriteLine(err.Message);
                continue;
            }
        }

            throw new CompilerError(stmtLoc, "expected statement");
        }

        ASTFunctionCall ParseFunctionCall() 
        {
            var identTok = Consume(TokenType.Identifier);
            Consume(TokenType.ParentStart);

            var ast = new ASTFunctionCall() 
            {
                identifier = identTok
            };

            while(!nextToken.Is(TokenType.ParentEnd)) 
            {
                ast.arguments.Add(ParseExpression());
                if(!nextToken.Is(TokenType.ParentEnd))
                    Consume(TokenType.Comma);
            }

            Consume(TokenType.ParentEnd);
            return ast;
        }

        ASTExpression ParseExpression()
        {
            if(nextToken.Is(TokenType.Integer)) {
                Consume();
                return new ASTInteger() { value = currToken };
            }
            if(nextToken.Is(TokenType.String)) {
                Consume();
                return new ASTString() { value = currToken };
            }
            if(nextToken.Is(TokenType.Identifier))
                return ParseFunctionCall();

            throw new CompilerError(currToken.end, "expected expression");
        }
    }
}