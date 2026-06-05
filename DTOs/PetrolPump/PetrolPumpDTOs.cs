using System;

namespace backend.DTOs.PetrolPump
{
    public class DashboardStatsDto
    {
        public int TotalLitresFilled { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalVehiclesServed { get; set; }
    }

    public class StockAmountDto
    {
        public decimal Petrol { get; set; }
        public decimal Diesel { get; set; }
        public decimal MobilOil { get; set; }
        public decimal EngineOil { get; set; }
        public decimal GearOil { get; set; }
        public decimal BreakOil { get; set; }
    }

    public class FuelEntryRequestDto
    {
        public string DdoCode { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public string Inventory { get; set; } = string.Empty; // Petrol, Diesel, etc.
        public decimal Litres { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateAllowance { get; set; }
    }

    public class FuelLogDto
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public decimal Litres { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FuelType { get; set; } = string.Empty;
    }

    public class VehicleSearchDto
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string DdoCode { get; set; } = string.Empty;
    }
}
