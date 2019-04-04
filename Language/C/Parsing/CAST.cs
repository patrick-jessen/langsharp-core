using System.Collections.Generic;
using Compiler.Lexing;
using Compiler.Parsing;

namespace Language.C.Parsing
{
    public class ASTFile : ASTNode
    {
        public List<ASTInclude> includes = new List<ASTInclude>();
        public List<ASTDeclaration> declarations = new List<ASTDeclaration>();
    }
    public class ASTInclude : ASTNode 
    { 
        public Token file; 
    }

    ////////////////////////////////////////////////////////////////////////////
    // Declarations
    ////////////////////////////////////////////////////////////////////////////
    public interface ASTDeclaration : ASTNode {}
    public class ASTFunction : ASTDeclaration
    {
        public Token name;
        public Token returns;
        public List<ASTStatement> statements = new List<ASTStatement>();
    }
    public class ASTExtern : ASTDeclaration
    {
        public ASTDeclaration declaration;
    }

    ////////////////////////////////////////////////////////////////////////////
    // Statements
    ////////////////////////////////////////////////////////////////////////////
    public interface ASTStatement : ASTNode {}
    public class ASTReturn : ASTStatement
    {
        public ASTExpression value;
    }
    class ASTFunctionCall : ASTStatement, ASTExpression
    {
        public Token identifier;
        public List<ASTExpression> arguments = new List<ASTExpression>();
    }

    ////////////////////////////////////////////////////////////////////////////
    // Expressions
    ////////////////////////////////////////////////////////////////////////////
    public interface ASTExpression : ASTNode {}
    public class ASTInteger : ASTExpression
    {
        public Token value;
    }
    public class ASTString : ASTExpression
    {
        public Token value;
    }
}