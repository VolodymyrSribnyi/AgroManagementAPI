using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs.V1.Machine;
using AgroManagementAPI.DTOs.V1.Worker;
using AgroManagementAPI.DTOs.V1.WorkerTask;

namespace AgroManagementAPI.DTOs.V1.FieldDetails;

public class FieldDetailsResponseDto
{
    public int Id { get; set; }
    public double Area { get; set; }
    public CultureType Culture { get; set; }
    public FieldStatus Status { get; set; }
    public List<WorkerResponseDto>? Workers { get; set; } = new();
    public List<MachineResponseDto>? Machines { get; set; } = new();
    public List<WorkerTaskResponseDto>? Tasks { get; set; } = new();
    public int RequiredWorkers { get; set; }
    public int RequiredMachines { get; set; }
    public double SeedAmount { get; set; }
    public double FertilizerAmount { get; set; }
    public double Yield { get; set; }
    public double FuelNeeded { get; set; }
    public double WorkDuralityNeeded { get; set; }
    public DateTime CreatedAt { get; set; }
}
