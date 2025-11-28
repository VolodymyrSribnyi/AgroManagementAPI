using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V1.Field;

public class FieldUpdateDto
{
    public double Area { get; set; }
    public CultureType Culture { get; set; }
    public FieldStatus Status { get; set; }
}
