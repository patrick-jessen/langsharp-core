namespace Language.C.Lexing
{
    public enum TokenType
    { 
        Keyword, Identifier, ParentStart, ParentEnd, 
        BraceStart, BraceEnd, Hash, String, Semicolon, 
        Integer, Float, Comma, EOF 
    };
}