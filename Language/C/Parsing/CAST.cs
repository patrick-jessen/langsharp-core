using System.Collections.Generic;
using Compiler.Lexing;
using Compiler.Parsing;

namespace Language.C.Parsing
{
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