using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V1.Machine;

public class MachineResponseDto
{
    public int Id { get; set; }
    public MachineType Type { get; set; }
    public double FuelConsumption { get; set; }
    public bool IsAvailable { get; set; }
    public double WorkDuralityPerHectare { get; set; }
    public int? FieldId { get; set; }
    public int ResourceId { get; set; }
}
