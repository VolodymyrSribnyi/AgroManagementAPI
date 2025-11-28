namespace AgroManagementAPI.DTOs.V1.Worker;

public class WorkerCreateDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int Age { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public decimal HoursWorked { get; set; }
}
