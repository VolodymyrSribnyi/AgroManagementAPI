using System.Collections.Generic;

namespace AgroindustryManagementAPI.Models;

public class Resource
{
    public Resource()
    {
    }
    public Resource(List<Machine> requiredMachines)
    {
        RequiredMachines = requiredMachines;
    }

    public int Id { get; set; }
    public CultureType CultureType { get; set; }
    public double SeedPerHectare { get; set; }
    public double FertilizerPerHectare { get; set; }
    public double WorkerPerHectare { get; set; }
    public double WorkerWorkDuralityPerHectare { get; set; }
    public double Yield {  get; set; }
    public List<Machine> RequiredMachines { get; set; } = [];
}