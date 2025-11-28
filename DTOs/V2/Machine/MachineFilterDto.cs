using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V2. Machine
{
    public class MachineFilterDto
    {
        public MachineType? Type { get; set; }
        public bool?  IsAvailable { get; set; }
        public double? MinFuelConsumption { get; set; }
        public double? MaxFuelConsumption { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}