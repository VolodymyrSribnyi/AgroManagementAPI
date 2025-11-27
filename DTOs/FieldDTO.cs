using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs
{
    public class FieldDTO
    {
        public double Area { get; set; }
        public CultureType Culture { get;set; }

        public FieldStatus Status { get; set; }
        public List<WorkerDTO>? Workers { get; set; } = new(); 
        public List<MachineDTO>? Machines { get; set; } = new();
        public List<WorkerTaskDTO>? Tasks { get; set; } = new();
    }
}
