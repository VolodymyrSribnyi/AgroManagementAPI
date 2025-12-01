namespace AgroManagementAPI.DTOs.V1.InventoryItem;

public class InventoryItemResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Quantity { get; set; }
    public required string Unit { get; set; }
}
