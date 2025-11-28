namespace AgroManagementAPI.DTOs.V1.InventoryItem;

public class InventoryItemUpdateDto
{
    public int WarehouseId { get; set; }
    public required string Name { get; set; }
    public int Quantity { get; set; }
    public required string Unit { get; set; }
}
