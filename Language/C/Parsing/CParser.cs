using Compiler;
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

            switch(nextToken.type) 
            {
                case TokenType.Keyword:
                    ast.declarations.Add(ParseFunction());
                    break;
            }

            return ast;
        }

        private ASTInclude ParseInclude() 
        {
            Consume(TokenType.Hash);
            Consume(TokenType.Keyword, "include");
            var fileTok = Consume(TokenType.String);
            return new ASTInclude() { file = fileTok };
        }

        private ASTFunction ParseFunction() 
        {

            var retTok = Consume(TokenType.Keyword);
            var nameTok = Consume(TokenType.Identifier);
            Consume(TokenType.ParentStart);
            Consume(TokenType.ParentEnd);
            Consume(TokenType.BraceStart);

            var ast = new ASTFunction() 
            {
                name = nameTok,
                returns = retTok
            };

            while(!nextToken.Is(TokenType.BraceEnd))
            {
                ast.statements.Add(ParseStatement());
                Consume(TokenType.Semicolon);
            }
            Consume(TokenType.BraceEnd);

            return ast;
        }

        ASTStatement ParseStatement() 
        {
            if(nextToken.Is(TokenType.Keyword))
            {
                var keywordTok = Consume();
                if(keywordTok.value == "return")
                {
                    return new ASTReturn()
                    {
                        value = ParseExpression()
                    };
                }
            }
            else if(nextToken.Is(TokenType.Identifier))
            {
                var identTok = Consume();
                if(nextToken.Is(TokenType.ParentStart))
                {
                    var ast = new ASTFunctionCall()
                    {
                        identifier = identTok
                    };

                    Consume();
                    while(!nextToken.Is(TokenType.ParentEnd)) 
                    {
                        ast.arguments.Add(ParseExpression());
                        if(nextToken.Is(TokenType.Comma))
                            Consume();
                    }

                    Consume();
                    return ast;
                }
            }
            throw new CompilerException(nextToken.start, "expected statement");
        }

        ASTExpression ParseExpression()
        {
            Consume();
            if(currToken.Is(TokenType.Integer))
                return new ASTInteger() { value = currToken };
            if(currToken.Is(TokenType.String))
                return new ASTString() { value = currToken };
            throw new CompilerException(currToken.start, "expected expression");
        }
    }
}