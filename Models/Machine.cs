namespace AgroindustryManagementAPI.Models;

public class Machine
{
    public int Id { get; set; }
    public MachineType Type { get; set; }
    public double FuelConsumption { get; set; }
    public bool IsAvailable { get; set; }
    public double WorkDuralityPerHectare {  get; set; }
    public int? FieldId { get; set; }
    public virtual Field? Field { get; set; }
    public int ResourceId { get; set; }
    public virtual Resource? Resource { get; set; }
}

