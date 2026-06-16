using System;

namespace DavonAllen.ContentPipelineDemo
{
    public sealed class PipelineDiagnostic
    {
        public PipelineDiagnostic(DiagnosticSeverity severity, string code, string message, int lineNumber, string fieldName)
        {
            Severity = severity;
            Code = code ?? string.Empty;
            Message = message ?? string.Empty;
            LineNumber = lineNumber;
            FieldName = fieldName ?? string.Empty;
        }

        public DiagnosticSeverity Severity { get; private set; }
        public string Code { get; private set; }
        public string Message { get; private set; }
        public int LineNumber { get; private set; }
        public string FieldName { get; private set; }

        public override string ToString()
        {
            string location = LineNumber > 0 ? "Line " + LineNumber : "File";
            string field = string.IsNullOrEmpty(FieldName) ? string.Empty : " [" + FieldName + "]";
            return Severity + " " + Code + " - " + location + field + ": " + Message;
        }
    }
}
