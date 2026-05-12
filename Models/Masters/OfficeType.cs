using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Masters
{
    public class OfficeType
    {
        // Primary Key - matching legacy OfficeTypeId
        public int OfficeTypeId { get; set; }
        
        // Legacy-compatible field names
        public string OfficeTypeName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
