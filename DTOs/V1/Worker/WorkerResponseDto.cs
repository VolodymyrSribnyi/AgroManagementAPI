using AgroManagementAPI.DTOs.V1.WorkerTask;

namespace AgroManagementAPI.DTOs.V1.Worker;

public class WorkerResponseDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Age { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public List<WorkerTaskResponseDto>? Tasks { get; set; } = new();
    public decimal HoursWorked { get; set; }
}
