using System;
using Compiler;
using Compiler.Lexing;
using Compiler.Parsing;
using Compiler.Source;
using Language.C.Lexing;

namespace Language.C.Parsing
{
    public class CParser : Parser 
    {
        public CParser(SourceFile file) : base(file, new CLexer(file)) {}

        protected override ASTNode ParseOne()
        {
            var ast = new ASTFile();

            while(nextToken.Is(CTokens.Hash))
                ast.includes.Add(ParseInclude());

            while(!nextToken.Is(Token.EOF))
                ast.declarations.Add(ParseDeclaration());

            return ast;
        }

        private ASTInclude ParseInclude() 
        {
            try 
            {
                Consume(CTokens.Hash);
                Consume(CTokens.Keyword, "include");
                var fileTok = Consume(CTokens.String);
                return new ASTInclude() { file = fileTok };
            }
            catch 
            {
                ReportError(currToken.end, "expected include statement");
                return null;
            }
        }

        private ASTDeclaration ParseDeclaration()
        {
            if(nextToken.Is(CTokens.Keyword, "extern"))
                return ParseExtern();
            if(nextToken.Is(CTokens.Keyword))
                return ParseFunction();

            ConsumeAndReportError(nextToken.start, "expected declaration");
            return null;
        }

        private ASTExtern ParseExtern() 
        {
            var ast = new ASTExtern();

            Consume(CTokens.Keyword, "extern");
            ast.declaration = ParseFunctionSignature();
            Consume(CTokens.Semicolon);
            return ast;
        }

        private ASTFunction ParseFunctionSignature()
        {
            var retTok = Consume(CTokens.Keyword);
            var nameTok = Consume(CTokens.Identifier);
            Consume(CTokens.ParentStart);
            Consume(CTokens.ParentEnd);

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
                Consume(CTokens.BraceStart);
                while(!nextToken.Is(CTokens.BraceEnd) && !nextToken.Is(Token.EOF))
                {
                    ast.statements.Add(ParseStatement());
                    Consume(CTokens.Semicolon);
                }
                Consume(CTokens.BraceEnd);
            }
            catch {
                ReportError(currToken.end, "expected function body");
                return null;
            }

            return ast;
        }

        ASTStatement ParseStatement() 
        {
            var stmtLoc = nextToken.start;

            while(!nextToken.Is(Token.EOF)) {
                try {
                    if(nextToken.Is(CTokens.Keyword, "return"))
                    {
                        Consume();
                        return new ASTReturn()
                        {
                            value = ParseExpression()
                        };
                    }
                    else if(nextToken.Is(CTokens.Identifier))
                        return ParseFunctionCall();
                    else
                        Consume();
                }
                catch {
                    ReportError(stmtLoc, "expected statement");
                    continue;
                }
            }

            ReportError(stmtLoc, "expected statement");
            return null;
        }

        ASTFunctionCall ParseFunctionCall() 
        {
            var identTok = Consume(CTokens.Identifier);
            Consume(CTokens.ParentStart);

            var ast = new ASTFunctionCall() 
            {
                identifier = identTok
            };

            if(!nextToken.Is(CTokens.ParentEnd))
            {
                while(true)
                {
                    ast.arguments.Add(ParseExpression());
                    if(nextToken.Is(CTokens.Comma))
                        Consume(CTokens.Comma);
                    else break;
                }
            }

            Consume(CTokens.ParentEnd);
            return ast;
        }

        ASTExpression ParseExpression()
        {
            if(nextToken.Is(CTokens.Integer)) {
                Consume();
                return new ASTInteger() { value = currToken };
            }
            if(nextToken.Is(CTokens.String)) {
                Consume();
                return new ASTString() { value = currToken };
            }
            if(nextToken.Is(CTokens.Identifier))
                return ParseFunctionCall();

            ReportError(currToken.end, "expected expression");
            return null;
        }
    }
}