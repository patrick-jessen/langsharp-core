using System;
using System.Collections.Generic;
using parser;

namespace langsharp_core 
{
    public enum Token
    { 
        Keyword, Identifier, ParentStart, ParentEnd, 
        BraceStart, BraceEnd, Hash, String, Semicolon, 
        Integer, Float, Comma, EOF 
    };

    public class CLexer : lexer.Lexer
    {
        public CLexer(string fileName) : base(fileName) {}

        protected override Enum LexOne(char first)
        {
            // Ignore whitespace
            if (Char.IsWhiteSpace(first))
            {
                while(!EOF && Char.IsWhiteSpace(Peek())) Consume();

                // Ignore white space
                skipToken = true;
                return null;
            }
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
            // Lex comments
            if(first == '/')
            {
                var second = Consume();
                if(second == '/')
                {
                    while(!EOF && Peek() != '\n') Consume();
                    skipToken = true;
                    return null;
                }
                if(second == '*')
                {
                    var v = Consume();
                    while(!EOF && (v != '*' || Peek() != '/')) v = Consume();
                    Consume();
                    skipToken = true;
                    return null;
                }
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
                case ',': return Token.Comma;              
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

    public class CParser : parser.Parser 
    {
        public CParser(string fileName) : base(new CLexer(fileName)) {}

        protected override ASTNode ParseOne()
        {
            var ast = new ASTFile();

            while(nextToken.Is(Token.Hash))
                ast.includes.Add(ParseInclude());

            switch(nextToken.type) 
            {
                case Token.Keyword:
                    ast.declarations.Add(ParseFunction());
                    break;
            }

            return ast;
        }

        private ASTInclude ParseInclude() 
        {
            Consume(Token.Hash);
            Consume(Token.Keyword, "include");
            var fileTok = Consume(Token.String);
            return new ASTInclude() { file = fileTok };
        }

        private ASTFunction ParseFunction() 
        {

            var retTok = Consume(Token.Keyword);
            var nameTok = Consume(Token.Identifier);
            Consume(Token.ParentStart);
            Consume(Token.ParentEnd);
            Consume(Token.BraceStart);

            var ast = new ASTFunction() 
            {
                name = nameTok,
                returns = retTok
            };

            while(!nextToken.Is(Token.BraceEnd))
            {
                ast.statements.Add(ParseStatement());
                Consume(Token.Semicolon);
            }
            Consume(Token.BraceEnd);

            return ast;
        }

        ASTStatement ParseStatement() 
        {
            if(nextToken.Is(Token.Keyword))
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
            else if(nextToken.Is(Token.Identifier))
            {
                var identTok = Consume();
                if(nextToken.Is(Token.ParentStart))
                {
                    var ast = new ASTFunctionCall()
                    {
                        identifier = identTok
                    };

                    Consume();
                    while(!nextToken.Is(Token.ParentEnd)) 
                    {
                        ast.arguments.Add(ParseExpression());
                        if(nextToken.Is(Token.Comma))
                            Consume();
                    }

                    Consume();
                    return ast;
                }
            }
            throw new parser.Exception(nextToken.start, "expected statement");
        }

        ASTExpression ParseExpression()
        {
            Consume();
            if(currToken.Is(Token.Integer))
                return new ASTInteger() { value = currToken };
            if(currToken.Is(Token.String))
                return new ASTString() { value = currToken };
            throw new parser.Exception(currToken.start, "expected expression");
        }

        class ASTFile : ASTNode
        {
            public List<ASTInclude> includes = new List<ASTInclude>();
            public List<ASTDeclaration> declarations = new List<ASTDeclaration>();
        }
        class ASTInclude : ASTNode 
        { 
            public lexer.Token file; 
        }
        interface ASTDeclaration : ASTNode {}
        class ASTFunction : ASTDeclaration
        {
            public lexer.Token name;
            public lexer.Token returns;
            public List<ASTStatement> statements = new List<ASTStatement>();
        }
    
        interface ASTStatement : ASTNode {}
        class ASTReturn : ASTStatement
        {
            public ASTExpression value;
        }
        class ASTFunctionCall : ASTStatement
        {
            public lexer.Token identifier;
            public List<ASTExpression> arguments = new List<ASTExpression>();
        }

        interface ASTExpression : ASTNode {}
        class ASTInteger : ASTExpression
        {
            public lexer.Token value;
        }
        class ASTString : ASTExpression
        {
            public lexer.Token value;
        }
    }
}