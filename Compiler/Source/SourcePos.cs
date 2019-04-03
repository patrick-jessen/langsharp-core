using System;

namespace Compiler.Source
{
    public class SourcePos
    {
        public int lineNo;      // Line number (1-based)
        public int columnNo;    // Column number (1-based)

        public SourcePos(int lineNo, int columnNo)
        {
            this.lineNo = lineNo;
            this.columnNo = columnNo;
        }
    }
}