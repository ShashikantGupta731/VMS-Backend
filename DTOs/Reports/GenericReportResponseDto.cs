using System.Collections.Generic;

namespace backend.DTOs.Reports
{
    public class GenericReportResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new List<string>();
        public List<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
    }
}
