using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V1.Field;

public class FieldCreateDto
{
    public double Area { get; set; }
    public CultureType Culture { get; set; }
    public FieldStatus Status { get; set; }
}
