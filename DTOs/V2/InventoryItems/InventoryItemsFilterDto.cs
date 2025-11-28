namespace AgroManagementAPI.DTOs.V2.InventoryItem
{
    public class InventoryItemFilterDto
    {
        public string? Name { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public int? WarehouseId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}