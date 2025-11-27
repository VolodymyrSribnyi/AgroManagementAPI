using System;
using AgroindustryManagementAPI.Models;

namespace AgroindustryManagementAPI.Services.Calculations;

public interface IAGCalculationService
{
    /// <summary>
    /// Calculates the required amount of seeds for a given crop and area (in kg or tons).
    /// </summary>
    double CalculateSeedAmount(Resource resource/*CultureType cropType*/, double areaInHectares);

    /// <summary>
    /// Calculates the amount of fertilizers or plant protection products needed for the area.
    /// </summary>
    double CalculateFertilizerAmount(Resource resource/*CultureType cropType*/, double areaInHectares);

    /// <summary>
    /// Estimates the potential yield for a given area, depending on the crop and weather conditions.
    /// </summary>
    double EstimateYield(Resource resource/*CultureType cropType*/, double areaInHectares);

    /// <summary>
    /// Determines the required number of machines to process a specific area.
    /// </summary>
    int CalculateRequiredMachineryCount(Resource resource);

    /// <summary>
    /// Estimates fuel consumption for field processing.
    /// </summary>
    double EstimateFuelConsumption(Machine machine, double areaInHectares);

    /// <summary>
    /// Determines the required number of workers to complete the task.
    /// </summary>
    int CalculateRequiredWorkers(Resource resource, double areaInHectares);

    /// <summary>
    /// Calculates the duration of work (in hours or days) for a specific area.
    /// </summary>
    double EstimateWorkDuration(double areaInHectares, int workersCount, Machine machine, Resource resource);

    /// <summary>
    /// Calculates the worker's bonus for exceeding the plan.
    /// </summary>
    decimal CalculateBonus(Worker worker, List<WorkerTask>tasks);

    // /// <summary>
    // /// Calculates the worker's efficiency coefficient.
    // /// </summary>
    // double CalculateWorkerEfficiency(double plannedWork, double completedWork, TimeSpan actualTime);
    //
    // /// <summary>
    // /// Estimates the cost of all resources for a given field.
    // /// </summary>
    // double CalculateFieldCost(CultureType cropType, double areaInHectares);
    //
    // /// <summary>
    // /// Calculates the projected profit considering costs and yield.
    // /// </summary>
    // double EstimateProfit(CultureType cropType, double areaInHectares, double marketPricePerTon);
}