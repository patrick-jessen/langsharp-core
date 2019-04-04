using Compiler.Lexing;

namespace Language.C.Lexing
{
    class CTokens
    {
        public static TokenType Keyword         = new TokenType("keyword");
        public static TokenType Identifier      = new TokenType("identifier");
        public static TokenType ParentStart     = new TokenType("'('");
        public static TokenType ParentEnd       = new TokenType("')'");
        public static TokenType BraceStart      = new TokenType("'{'");
        public static TokenType BraceEnd        = new TokenType("'}'");
        public static TokenType Hash            = new TokenType("'#'");
        public static TokenType String          = new TokenType("string");
        public static TokenType Semicolon       = new TokenType("';'");
        public static TokenType Integer         = new TokenType("integer");
        public static TokenType Float           = new TokenType("float");
        public static TokenType Comma           = new TokenType("','");
    }
}