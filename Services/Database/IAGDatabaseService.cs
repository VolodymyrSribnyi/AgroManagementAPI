using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AgroindustryManagementAPI.Services.Database;

/// <summary>
/// Interface for database service that provides CRUD operations and utility methods for managing fields, workers, machines, inventory items, and worker tasks.
/// </summary>
public interface IAGDatabaseService
{
    #region CRUD operations for Fields

    /// <summary>
    /// Retrieves a field by its unique identifier.
    /// </summary>
    /// <param name="fieldId">The unique identifier of the field.</param>
    /// <returns>The field with the specified ID.</returns>
    Field GetFieldById(int fieldId);

    /// <summary>
    /// Retrieves all fields.
    /// </summary>
    /// <returns>A collection of all fields.</returns>
    IEnumerable<Field> GetAllFields();

    /// <summary>
    /// Adds a new field to the database.
    /// </summary>
    /// <param name="field">The field to add.</param>
    Field AddField(FieldDTO field);

    /// <summary>
    /// Updates an existing field in the database.
    /// </summary>
    /// <param name="field">The field with updated information.</param>
    void UpdateField(Field field);

    /// <summary>
    /// Deletes a field by its unique identifier.
    /// </summary>
    /// <param name="fieldId">The unique identifier of the field to delete.</param>
    void DeleteField(int fieldId);
    #endregion
    #region CRUD operations for Workers

    /// <summary>
    /// Retrieves a worker by their unique identifier.
    /// </summary>
    /// <param name="workerId">The unique identifier of the worker.</param>
    /// <returns>The worker with the specified ID.</returns>
    Worker GetWorkerById(int workerId);

    /// <summary>
    /// Retrieves all workers.
    /// </summary>
    /// <returns>A collection of all workers.</returns>
    IEnumerable<Worker> GetAllWorkers();

    /// <summary>
    /// Adds a new worker to the database.
    /// </summary>
    /// <param name="worker">The worker to add.</param>
    void AddWorker(Worker worker);

    /// <summary>
    /// Updates an existing worker in the database.
    /// </summary>
    /// <param name="worker">The worker with updated information.</param>
    void UpdateWorker(Worker worker);

    /// <summary>
    /// Deletes a worker by their unique identifier.
    /// </summary>
    /// <param name="workerId">The unique identifier of the worker to delete.</param>
    void DeleteWorker(int workerId);
    #endregion

    #region CRUD operations for Machines

    /// <summary>
    /// Retrieves a machine by its unique identifier.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine.</param>
    /// <returns>The machine with the specified ID.</returns>
    Machine GetMachineById(int machineId);

    /// <summary>
    /// Retrieves all machines.
    /// </summary>
    /// <returns>A collection of all machines.</returns>
    IEnumerable<Machine> GetAllMachines();

    /// <summary>
    /// Adds a new machine to the database.
    /// </summary>
    /// <param name="machine">The machine to add.</param>
    void AddMachine(Machine machine);

    /// <summary>
    /// Updates an existing machine in the database.
    /// </summary>
    /// <param name="machine">The machine with updated information.</param>
    void UpdateMachine(Machine machine);

    /// <summary>
    /// Deletes a machine by its unique identifier.
    /// </summary>
    /// <param name="machineId">The unique identifier of the machine to delete.</param>
    void DeleteMachine(int machineId);
    #endregion
    #region CRUD operations for Inventory Items

    /// <summary>
    /// Retrieves an inventory item by its unique identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the inventory item.</param>
    /// <returns>The inventory item with the specified ID.</returns>
    InventoryItem GetInventoryItemById(int itemId);

    /// <summary>
    /// Retrieves all inventory items.
    /// </summary>
    /// <returns>A collection of all inventory items.</returns>
    IEnumerable<InventoryItem> GetAllInventoryItems();

    /// <summary>
    /// Adds a new inventory item to the database.
    /// </summary>
    /// <param name="item">The inventory item to add.</param>
    void AddInventoryItem(InventoryItem item);

    /// <summary>
    /// Updates an existing inventory item in the database.
    /// </summary>
    /// <param name="item">The inventory item with updated information.</param>
    void UpdateInventoryItem(InventoryItem item);

    /// <summary>
    /// Deletes an inventory item by its unique identifier.
    /// </summary>
    /// <param name="itemId">The unique identifier of the inventory item to delete.</param>
    void DeleteInventoryItem(int itemId);
    #endregion
    #region CRUD operations for Worker Tasks

    /// <summary>
    /// Retrieves a worker task by its unique identifier.
    /// </summary>
    /// <param name="taskId">The unique identifier of the worker task.</param>
    /// <returns>The worker task with the specified ID.</returns>
    WorkerTask GetWorkerTaskById(int taskId);

    /// <summary>
    /// Retrieves all worker tasks.
    /// </summary>
    /// <returns>A collection of all worker tasks.</returns>
    IEnumerable<WorkerTask> GetAllWorkerTasks(string searchDate = null);

    /// <summary>
    /// Adds a new worker task to the database.
    /// </summary>
    /// <param name="task">The worker task to add.</param>
    void AddWorkerTask(WorkerTask task);

    /// <summary>
    /// Updates an existing worker task in the database.
    /// </summary>
    /// <param name="task">The worker task with updated information.</param>
    void UpdateWorkerTask(WorkerTask task);

    /// <summary>
    /// Deletes a worker task by its unique identifier.
    /// </summary>
    /// <param name="taskId">The unique identifier of the worker task to delete.</param>
    void DeleteWorkerTask(int taskId);
    #endregion

    Resource GetResourceById(int resourceId);

    
    IEnumerable<Resource> GetAllResources();

    void AddResource(Resource resource);

   
    void UpdateResource(Resource resource);

    
    void DeleteResource(int resourceId);

    Warehouse GetWarehouseById(int warehouseId);

    IEnumerable<Warehouse> GetAllWarehouses();

    void AddWarehouse(Warehouse warehouse);

    void UpdateWarehouse(Warehouse warehouse);
    void DeleteWarehouse(int warehouseId);
    // Additional utility methods
    Resource GetResourceByCultureType(CultureType cultureType);
    Machine GetMachineByMachineType(MachineType machineType);
    /// <summary>
    /// Retrieves inventory items that are critically low in stock.
    /// </summary>
    /// <returns>A collection of inventory items with low stock levels.</returns>
    
    IEnumerable<InventoryItem> GetCriticalInventoryItems();

    /// <summary>
    /// Retrieves tasks assigned to a specific worker.
    /// </summary>
    /// <param name="workerId">The unique identifier of the worker.</param>
    /// <returns>A collection of tasks assigned to the specified worker.</returns>
    IEnumerable<WorkerTask> GetTasksByWorkerId(int workerId);

    /// <summary>
    /// Retrieves machines that are currently available for use.
    /// </summary>
    /// <returns>A collection of available machines.</returns>
    IEnumerable<Machine> GetAvailableMachines();
}