using System;

namespace Compiler.Lexing
{
    public class Location
    {
        public string file;     // Name of the file
        public int lineNo;      // Line number (1-based)
        public int columnNo;    // Column number (1-based)

        public Location(string file, int lineNo, int columnNo)
        {
            this.file = file;
            this.lineNo = lineNo;
            this.columnNo = columnNo;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", file, lineNo, columnNo);
        }
    }
}