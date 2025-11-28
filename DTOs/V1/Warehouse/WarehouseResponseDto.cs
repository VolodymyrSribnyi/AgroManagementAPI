using AgroManagementAPI.DTOs.V1.InventoryItem;

namespace AgroManagementAPI.DTOs.V1.Warehouse;

public class WarehouseResponseDto
{
    public int Id { get; set; }
    public List<InventoryItemResponseDto>? InventoryItems { get; set; } = new();
}
