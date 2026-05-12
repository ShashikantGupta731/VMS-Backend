using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Masters
{
    public class Allocation
    {
        // Primary Key - matching legacy AllocationTypeId
        [Key]
        public int AllocationTypeId { get; set; }
        
        // Legacy-compatible field names
        public string AllocationTypeName { get; set; } = string.Empty;
        
        // Legacy date fields
        public DateTime? PDate { get; set; }
        public DateTime? TDate { get; set; }
    }
}
