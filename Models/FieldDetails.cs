using AgroindustryManagementAPI.Models;

namespace AgroManagementAPI.Models
{
    public class FieldDetails
    {
        public int Id { get; set; }
        public double Area { get; set; }
        public CultureType Culture { get; set; }
        public FieldStatus Status { get; set; }
        public List<Worker>? Workers { get; set; } = new();
        public List<Machine>? Machines { get; set; } = new();
        public List<WorkerTask>? Tasks { get; set; } = new();
        public int RequiredWorkers { get; set; }
        public int RequiredMachines { get; set; }
        public double SeedAmount { get; set; }
        public double FertilizerAmount { get; set; }
        public double Yield { get; set; }
        public double FuelNeeded { get; set; }
        public double WorkDuralityNeeded { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
