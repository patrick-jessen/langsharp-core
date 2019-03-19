using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lexer;

namespace parser
{
    public abstract class Parser
    {
        private Lexer lexer;
        protected Token currToken;
        protected Token nextToken;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            nextToken = lexer.Lex();
            currToken = new Token(nextToken.start, nextToken.start, null, null);
        }

        public ASTNode Parse() 
        {
            ASTNode ast = ParseOne();
            Consume(Token.Type.EOF); // Make sure we are at EOF
            return ast;
        }

        protected abstract ASTNode ParseOne();

        protected Token Consume(Enum type = null, String value = null)
        {
            if(type != null && !nextToken.Is(type))
                throw new Exception(currToken.end, String.Format("expected '{0}'", type));
            if(value != null && nextToken.value != value)
                throw new Exception(currToken.end, String.Format("expected '{0}'", value));

            currToken = nextToken;
            nextToken = lexer.Lex();
            return currToken;
        }

        public static string Indent(int indent, string str)
        {
            string end = "";
            if (str.EndsWith("\n"))
            {
                end = "\n";
                str = str.TrimEnd('\n');
            }
            var indentStr = new String(' ', indent * 2);
            var separator = "\n" + indentStr;
            return indentStr + String.Join(separator, str.Split('\n')) + end;
        }
    }
}