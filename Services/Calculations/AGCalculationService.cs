using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Models;

namespace AgroindustryManagementAPI.Services.Calculations;

public class AGCalculationService : IAGCalculationService
{
    // Why CalculationService should have access to database? 
    // CalculationService performs calculations rather than accessing the database
    // All data required for calculations should be passed as parameters to its methods.
    //private readonly IAGDatabaseService _databaseService;  
   /* public AGCalculationService(IAGDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }*/
    public double CalculateSeedAmount(Resource resource/*CultureType cropType*/, double areaInHectares)
    {
        if (areaInHectares <= 0)
        {
            throw new ArgumentException("Area in hectares must be greater than zero.");
        }
        
        //var resource = _databaseService.GetResourceByCultureType(cropType);


        /*if (resource == null)
        {
            //throw new KeyNotFoundException($"No resource found for crop type: {cropType}");
            throw new ArgumentException("Resource is null");
        }*/

        var seedAmount = resource.SeedPerHectare * areaInHectares;

        return seedAmount;
    }
    
    public double CalculateFertilizerAmount(Resource resource/*CultureType cropType*/, double areaInHectares)
    {
        if (areaInHectares <= 0)
        {
            throw new ArgumentException(nameof(areaInHectares), "Area in hectares must be greater than zero.");
        }
        //var resource = _databaseService.GetResourceByCultureType(cropType);
       /* if (resource == null)
        {
            // throw new KeyNotFoundException($"No resource found for crop type: {cropType}");
            throw new ArgumentException("Resource is null");
        }*/
        return resource.FertilizerPerHectare*areaInHectares;
    }

    public double EstimateYield(Resource resource/*CultureType cropType*/, double areaInHectares)
    {
        if (areaInHectares <= 0)
        {
            throw new ArgumentException(nameof(areaInHectares), "Area in hectares must be greater than zero.");
        }

        //var resource = _databaseService.GetResourceByCultureType(cropType);
        /*if(resource == null)
        {
            //throw new KeyNotFoundException($"No resource found for crop type: {cropType}");
            throw new ArgumentException("Resource is null");
        }*/
        return resource.Yield * areaInHectares;
    }

    public int CalculateRequiredMachineryCount(Resource resource/*CultureType cropType, double areaInHectares*/)
    {
        /*if (areaInHectares <= 0)
        {
            throw new ArgumentException(nameof(areaInHectares), "Area in hectares must be greater than zero.");
        }*/

       // var resource = _databaseService.GetResourceByCultureType(cropType);

       /* if (resource == null)
        {
            //throw new KeyNotFoundException($"No resource found for crop type: {cropType}");
            throw new ArgumentException("Resource is null");
        }*/

        return resource.RequiredMachines.Count;
    }
     
    public double EstimateFuelConsumption(Machine machine/*MachineType machineType*/, double areaInHectares)
    {
        if (areaInHectares <= 0)
        {
            throw new ArgumentException(nameof(areaInHectares), "Area in hectares must be greater than zero.");
        }
        //var concreteMachine=_databaseService.GetMachineByMachineType(machineType);
        return machine.FuelConsumption * areaInHectares;
    }

    public int CalculateRequiredWorkers(Resource resource/*CultureType cropType*/, double areaInHectares)
    {
        if (areaInHectares <= 0)
        {
            throw new ArgumentException(nameof(areaInHectares), "Area in hectares must be greater than zero.");
        }

        //var resource = _databaseService.GetResourceByCultureType(cropType);

        /*if (resource == null)
        {
            throw new KeyNotFoundException($"No resource found for crop type: {cropType}");
        }*/

        return (int)Math.Ceiling(resource.WorkerPerHectare * areaInHectares);
    }

    public double EstimateWorkDuration(double areaInHectares, int workersCount, /*MachineType machineryType, CultureType cropType*/Machine machine, Resource resource)
    {
        if (areaInHectares <= 0 || workersCount<=0)
        { 
            throw new ArgumentException("Area in hectares and workers count must be greater than zero.");
        }
        //var resource = _databaseService.GetResourceByCultureType(cropType);
        //var concreteMachine = _databaseService.GetMachineByMachineType(machineryType);
        /*if(resource == null || concreteMachine==null)
        {
            throw new KeyNotFoundException($"No machine are found");
        }*/
        double durationOfWorkerWork = resource.WorkerWorkDuralityPerHectare / workersCount * areaInHectares;
        double durationOfMachineWork = machine.WorkDuralityPerHectare * areaInHectares;
        return durationOfWorkerWork + durationOfMachineWork;
    }

    public decimal CalculateBonus(/*int workerId*/Worker worker, List<WorkerTask>tasks)
    {
        /*if (workerId <= 0)
        { 
            throw new ArgumentException("Worker id must be greater than zero. "); 
        }
        var worker = _databaseService.GetWorkerById(workerId);
        if (worker==null)
        {
            throw new KeyNotFoundException("Worker with such Id is not found");
        }*/
        decimal salary;
        //var tasks = _databaseService.GetTasksByWorkerId(workerId);
        decimal bonusPerDay = 0.02m;
        decimal sumOfBonuses = 0;
        foreach ( var task in tasks)
        {
            var differenceInDays = 0;
            if (task.RealEndDate.HasValue)
            {
                
                TimeSpan duration = task.EstimatesEndDate - task.RealEndDate.Value;
                differenceInDays = duration.Days;
            }
            else
            {
                
                differenceInDays = 0;
            }
            
            decimal bonus;
            if (differenceInDays<=0)
            {
                bonus = 0;
            }
            else
            {
                bonus=differenceInDays*bonusPerDay;
                if (bonus>0.06m)
                    bonus=0.06m;
                sumOfBonuses+=bonus;
            }
            if(bonus > 0.06m)
                bonus = 0.06m;
            sumOfBonuses += bonus;
        }
        salary=worker.HourlyRate*worker.HoursWorked;
        return salary * sumOfBonuses;
    }

    // UNIMPLEMENTED METHODS
    
    // public double CalculateWorkerEfficiency(double plannedWork, double completedWork, TimeSpan actualTime)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public double CalculateFieldCost(CultureType cropType, double areaInHectares)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public double EstimateProfit(CultureType cropType, double areaInHectares, double marketPricePerTon)
    // {
    //     throw new NotImplementedException();
    // }
}