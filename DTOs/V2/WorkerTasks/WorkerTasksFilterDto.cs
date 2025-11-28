using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V2.WorkerTask
{
    public class WorkerTaskFilterDto
    {
        public TaskType? TaskType { get; set; }
        public double? MinProgress { get; set; }
        public double? MaxProgress { get; set; }
        public int?  WorkerId { get; set; }
        public int? FieldId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}