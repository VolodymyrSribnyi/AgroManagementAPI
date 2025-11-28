namespace AgroManagementAPI.DTOs.V2.Worker
{
    public class WorkerFilterDto
    {
        public bool?  IsActive { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}