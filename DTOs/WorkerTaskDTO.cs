using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs
{
    public class WorkerTaskDTO
    {
        public string Description { get; set; }

        public int WorkerId { get; set; }
        public int FieldId { get; set; }
        public TaskType TaskType { get; set; }
        public double Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? RealEndDate { get; set; }
        public DateTime EstimatesEndDate { get; set; }
    }
}
