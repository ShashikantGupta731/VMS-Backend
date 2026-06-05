using System.Collections.Generic;

namespace backend.DTOs.Reports
{
    public class GenericReportRequestDto
    {
        public string ReportType { get; set; } = string.Empty;
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();
    }
}
