using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs
{
    public class WorkerDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
        public List<WorkerTaskDTO> Tasks { get; set; } = [];
        public decimal HoursWorked { get; set; }
    }
}
