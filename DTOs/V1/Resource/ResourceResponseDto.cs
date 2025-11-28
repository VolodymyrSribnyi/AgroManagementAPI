using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs.V1.Machine;

namespace AgroManagementAPI.DTOs.V1.Resource;

public class ResourceResponseDto
{
    public int Id { get; set; }
    public CultureType CultureType { get; set; }
    public double SeedPerHectare { get; set; }
    public double FertilizerPerHectare { get; set; }
    public double WorkerPerHectare { get; set; }
    public double WorkerWorkDuralityPerHectare { get; set; }
    public double Yield { get; set; }
    public List<MachineResponseDto>? RequiredMachines { get; set; } = new();
}
