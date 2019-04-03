

using System;
using System.Collections.Generic;

namespace Compiler.Source
{
    public class SourceFile 
    {
        public string path;
        public List<SourceError> errors = new List<SourceError>();

        public SourceFile(string path) 
        {
            this.path = path;
        }

        public void ReportError(SourceError err) 
        {
            errors.Add(err);
        }

        public String ErrorToString(SourceError err) 
        {
            return $"[ERROR] {err.message} at {path}:{err.pos.lineNo}:{err.pos.columnNo}";
        }
    }
}
