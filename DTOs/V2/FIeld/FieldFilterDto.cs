using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs. V2.Field
{
    public class FieldFilterDto
    {
        public CultureType? Culture { get; set; }
        public FieldStatus? Status { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}