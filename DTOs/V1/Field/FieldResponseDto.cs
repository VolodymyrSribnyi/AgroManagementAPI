using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs.V1.Worker;
using AgroManagementAPI.DTOs.V1.WorkerTask;

namespace AgroManagementAPI.DTOs.V1.Field;

public class FieldResponseDto
{
    public int Id { get; set; }
    public double Area { get; set; }
    public CultureType Culture { get; set; }
    public FieldStatus Status { get; set; }
    public List<WorkerResponseDto>? Workers { get; set; } = new();
    public List<WorkerTaskResponseDto>? Tasks { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
