using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs. V2.Resource
{
    public class ResourceFilterDto
    {
        public CultureType? CultureType { get; set; }
        public double? MinYield { get; set; }
        public double? MaxYield { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}