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

        public bool After(Location other)
        {
            if(lineNo > other.lineNo) return true;
            if(lineNo == other.lineNo && columnNo >= other.columnNo) return true;
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", file, lineNo, columnNo);
        }
    }
}