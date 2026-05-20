using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class TransferVehicleDto
    {
        [Required]
        public int VehicleId { get; set; }
        
        [Required]
        public int ToOfficeId { get; set; }
        
        [Required]
        public DateTime TransferDate { get; set; }
        
        [Required]
        public string TransferOrderNumber { get; set; } = string.Empty;
        
        public IFormFile? TransferOrderFile { get; set; }
        
        public string Remarks { get; set; } = string.Empty;

        // Destination Allocation & Assignment Details (Legacy Parity)
        public int? ToDeptId { get; set; }
        public string? ToAllocationType { get; set; }
        public int? ToDesignationId { get; set; }
        public int? ToOfficerId { get; set; }
        public string? ToHRMSCode { get; set; }
        public string? ToOfficerName { get; set; }
    }
}

