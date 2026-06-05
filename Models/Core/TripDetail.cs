using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models.Core
{
    [Table("TripDetails")]
    public class TripDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TripId { get; set; }

        [Required]
        public int VehicleInfoId { get; set; }

        [ForeignKey("VehicleInfoId")]
        public virtual VehicleInfo? VehicleInfo { get; set; }

        public double Longitude { get; set; }
        
        public double Latitude { get; set; }

        [MaxLength(255)]
        public string? DriverName { get; set; }

        [MaxLength(20)]
        public string? DriverPhone { get; set; }

        [MaxLength(50)]
        public string? OdometerReading { get; set; }

        public string? OdometerImgPath { get; set; }

        public bool TripCompleted { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
    }
}
