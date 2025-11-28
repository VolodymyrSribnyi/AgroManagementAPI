using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs.V1.Field;
using AgroManagementAPI.DTOs.V1.Worker;

namespace AgroManagementAPI.DTOs.V1.WorkerTask;

public class WorkerTaskResponseDto
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public int WorkerId { get; set; }
    public WorkerResponseDto? Worker { get; set; }
    public int FieldId { get; set; }
    public FieldResponseDto? Field { get; set; }
    public TaskType TaskType { get; set; }
    public double Progress { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? RealEndDate { get; set; }
    public DateTime EstimatesEndDate { get; set; }
}
