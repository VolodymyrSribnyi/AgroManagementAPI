namespace AgroindustryManagementAPI.Models;

public class InventoryItem
{
    public InventoryItem()
    {
    }
    public InventoryItem(string name, string unit)
    {
        Name = name;
        Unit = unit;
    }

    public int Id { get; set; }
    public virtual Warehouse Warehouse { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }

}