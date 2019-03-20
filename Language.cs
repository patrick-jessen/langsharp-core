using System;
using System.Collections.Generic;
using Compiler;
using Compiler.Lexing;
using Compiler.Parsing;

namespace langsharp_core 
{
    public enum TokenType
    { 
        Keyword, Identifier, ParentStart, ParentEnd, 
        BraceStart, BraceEnd, Hash, String, Semicolon, 
        Integer, Float, Comma, EOF 
    };

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
            }
            return false;
        }
    }

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

        class ASTFile : ASTNode
        {
            public List<ASTInclude> includes = new List<ASTInclude>();
            public List<ASTDeclaration> declarations = new List<ASTDeclaration>();
        }
        class ASTInclude : ASTNode 
        { 
            public Token file; 
        }
        interface ASTDeclaration : ASTNode {}
        class ASTFunction : ASTDeclaration
        {
            public Token name;
            public Token returns;
            public List<ASTStatement> statements = new List<ASTStatement>();
        }
    
        interface ASTStatement : ASTNode {}
        class ASTReturn : ASTStatement
        {
            public ASTExpression value;
        }
        class ASTFunctionCall : ASTStatement
        {
            public Token identifier;
            public List<ASTExpression> arguments = new List<ASTExpression>();
        }

        interface ASTExpression : ASTNode {}
        class ASTInteger : ASTExpression
        {
            public Token value;
        }
        class ASTString : ASTExpression
        {
            public Token value;
        }
    }
}