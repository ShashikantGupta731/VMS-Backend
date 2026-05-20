using System;
using System.Collections.Generic;

namespace backend.DTOs.Billing
{
    public class PersonalUsagePayloadDto
    {
        public string record_id { get; set; } = string.Empty;
        public string item_id { get; set; } = string.Empty;
        public int plan_id { get; set; }
        public int vehicle_info_id { get; set; }
        public string vehicle_no { get; set; } = string.Empty;
        public string personal_use_details { get; set; } = string.Empty; // The JSON string we will deserialize
    }

    public class PersonalUsageLogDto
    {
        public string OfficerId { get; set; } = string.Empty;
        public string DateOfUse { get; set; } = string.Empty;
        public decimal OMFrom { get; set; }
        public decimal OMTo { get; set; }
    }
}
