using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.DTOs.V1.Resource;

public class ResourceUpdateDto
{
    public CultureType CultureType { get; set; }
    public double SeedPerHectare { get; set; }
    public double FertilizerPerHectare { get; set; }
    public double WorkerPerHectare { get; set; }
    public double WorkerWorkDuralityPerHectare { get; set; }
    public double Yield { get; set; }
}
