using AgroindustryManagementAPI.Models;
using AgroManagementAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AgroManagementAPI.Models;

namespace AgroindustryManagementAPI.Services.Database;

public class AGDatabaseService : IAGDatabaseService
{
    private readonly AGDatabaseContext _context;
    public AGDatabaseService(AGDatabaseContext context)
    {
        _context = context;
    }
    public Field GetFieldById(int fieldId)
    {
        if (fieldId <= 0)
        {
            throw new ArgumentException("Field ID must be a positive integer.", nameof(fieldId));
        }
        var field = _context.Fields.Include(f => f.Workers)   
                                    .Include(f => f.Machines)  
                                    .Include(f => f.Tasks)
                                    .FirstOrDefault(field => field.Id == fieldId);

        if (field == null)
        {
            throw new KeyNotFoundException($"Field with ID {fieldId} not found.");
        }

        return field;
    }

    public IEnumerable<Field> GetAllFields()
    {
        var fields = _context.Fields
            .Include(f => f.Workers)
            .Include(f => f.Machines)
            .Include(f => f.Tasks)
            .ToList();;

        return fields.Count == 0 ? Enumerable.Empty<Field>() : fields;
    }

    public Field AddField(FieldDTO field)
    {
        var newField = new Field
        {
            Id = _context.Fields.Any() ? _context.Fields.Max(o => o.Id) + 1 : 1,
            Area = field.Area,
            Culture = field.Culture,
            Status = field.Status,
        };
        // test this
        //if (_context.Fields.FirstOrDefault(dbField => dbField.Id == field.Id) != null) 
        //    return;

        _context.Fields.Add(newField);
        _context.SaveChanges();
        return newField;
    }

    public void UpdateField(Field field)
    {
        var existingField = _context.Fields.FirstOrDefault(dbField => dbField.Id == field.Id);

        if (existingField == null)
        {
            throw new KeyNotFoundException($"Field with ID {field.Id} not found.");
        }
        existingField.Culture = field.Culture;
        existingField.Area = field.Area;
        existingField.Status = field.Status;
        existingField.Workers = field.Workers;
        existingField.Machines = field.Machines;
        existingField.Tasks = field.Tasks;

        _context.SaveChanges();
    }

    public void DeleteField(int fieldId)
    {
        if (fieldId <= 0)
        {
            throw new ArgumentException("Field ID must be a positive integer.", nameof(fieldId));
        }

        var field = _context.Fields.FirstOrDefault(dbField => dbField.Id == fieldId);

        if (field == null)
        {
            throw new KeyNotFoundException($"Field with ID {fieldId} not found.");
        }

        _context.Fields.Remove(field);
        _context.SaveChanges();
    }

    public Worker GetWorkerById(int workerId)
    {
        if (workerId <= 0)
        {
            throw new ArgumentException("Invalid id", nameof(workerId));
        }
        
        var worker = _context.Workers.Include(f => f.Tasks).FirstOrDefault(dbWorker => dbWorker.Id == workerId);
        if (worker == null)
        {
            throw new KeyNotFoundException("Worker with such id is not found");
        }
        
        return worker;
    }

    public IEnumerable<Worker> GetAllWorkers()
    {
        var workers = _context.Workers.ToList();

        return workers.Count == 0 ? Enumerable.Empty<Worker>() : workers;
    }

    public void AddWorker(Worker worker)
    {
        if (_context.Workers.FirstOrDefault(dbWorker => dbWorker.Id == worker.Id) != null) 
            return;
        
        _context.Workers.Add(worker);
        _context.SaveChanges();
    }

    public void UpdateWorker(Worker worker)
    {
        var existingWorker = _context.Workers.FirstOrDefault(dbWorker => dbWorker.Id == worker.Id);
        if (existingWorker == null)
        {
            throw new KeyNotFoundException("Worker is not found");
        }
        
        existingWorker.HoursWorked = worker.HoursWorked;
        existingWorker.HourlyRate = worker.HourlyRate;
        existingWorker.Age = worker.Age;
        existingWorker.IsActive = worker.IsActive;
        existingWorker.Tasks = worker.Tasks;
        existingWorker.FirstName = worker.FirstName;
        existingWorker.LastName = worker.LastName;
        
        _context.SaveChanges();
    }

    public void DeleteWorker(int workerId)
    {
        if (workerId <= 0)
        {
            throw new ArgumentException("Worker id must be positive", nameof(workerId));
        }
        
        var workerExist = _context.Workers.FirstOrDefault(dbWorker => dbWorker.Id == workerId);
        if (workerExist == null)
        {
            throw new KeyNotFoundException("Such worker is not found");
        }
        
        _context.Workers.Remove(workerExist);
        _context.SaveChanges();
    }

    public Machine GetMachineById(int machineId)
    {
        if (machineId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(machineId));
        }
        
        var machineExist = _context.Machines.Include(t=>t.Field).Include(t=>t.Resource).FirstOrDefault(dbMachine => dbMachine.Id == machineId);
        if (machineExist == null)
        {
            throw new KeyNotFoundException("Machine is not found");
        }
        
        return machineExist;
    }

    public IEnumerable<Machine> GetAllMachines()
    {
        var machines = _context.Machines.Include(t => t.Field).Include(t => t.Resource).ToList();
        return machines.Count == 0 ? Enumerable.Empty<Machine>() : machines;
    }
    public void AddMachine(Machine machine)
    {
        if (_context.Machines.FirstOrDefault(dbMachine => dbMachine.Id == machine.Id) != null) 
            return;
        
        _context.Machines.Add(machine);
        _context.SaveChanges();
    }

    public void UpdateMachine(Machine machine)
    {
        var existingMachine = _context.Machines.FirstOrDefault(dbMachine => dbMachine.Id == machine.Id);
        if(existingMachine == null)
        { 
            throw new KeyNotFoundException("Such machine is not found"); 
        }
        
        existingMachine.FieldId = machine.FieldId;
        existingMachine.ResourceId = machine.ResourceId;
        existingMachine.IsAvailable=machine.IsAvailable;
        existingMachine.Type= machine.Type;
        existingMachine.FuelConsumption= machine.FuelConsumption;
        existingMachine.WorkDuralityPerHectare= machine.WorkDuralityPerHectare;
        
        _context.SaveChanges();
    }

    public void DeleteMachine(int machineId)
    {
        if (machineId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(machineId));
        }
        
        var machine = _context.Machines.FirstOrDefault(dbMachine => dbMachine.Id == machineId);
        if (machine == null)
        {
            throw new KeyNotFoundException("Such machine is not found");
        }
        
        _context.Machines.Remove(machine);
        _context.SaveChanges();
    }

    public InventoryItem GetInventoryItemById(int itemId)
    {
        if(itemId <= 0)
        { 
            throw new ArgumentException("Id must be positive",nameof(itemId)); 
        }
        
        var item = _context.InventoryItems.FirstOrDefault(dbInventoryItem => dbInventoryItem.Id == itemId);
        if(item == null)
        {
            throw new KeyNotFoundException("Such item is not found");
        }
        
        return item;
    }

    public IEnumerable<InventoryItem> GetAllInventoryItems()
    {
        var items=_context.InventoryItems.ToList();
        return items.Count==0 ? Enumerable.Empty<InventoryItem>() : items;
    }

    public void AddInventoryItem(InventoryItem item)
    {
        if (_context.InventoryItems.FirstOrDefault(dbInventoryItem => dbInventoryItem.Id == item.Id) != null) 
            return;
        
        _context.InventoryItems.Add(item);
        _context.SaveChanges();
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        var existingItem=_context.InventoryItems.FirstOrDefault(dbInventoryItem => dbInventoryItem.Id == item.Id);
        if (existingItem == null)
        {
            throw new KeyNotFoundException("There is no such item");
        }
        
        existingItem.Name = item.Name;
        existingItem.Quantity = item.Quantity;
        existingItem.Unit = item.Unit;
        existingItem.Warehouse = item.Warehouse;
        
        _context.SaveChanges();
    }

    public void DeleteInventoryItem(int itemId)
    {
        if(itemId<=0)
        {
            throw new ArgumentException("Id must be positive", nameof(itemId));
        }
        
        var item= _context.InventoryItems.FirstOrDefault(dbInventoryItem => dbInventoryItem.Id == itemId);
        if (item == null)
        {
            throw new KeyNotFoundException("There is no such item");
        }
        
        _context.InventoryItems.Remove(item);
        _context.SaveChanges();
    }

    public WorkerTask GetWorkerTaskById(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentException("ID must be positive", nameof(taskId));
        }
        
        var task = _context.WorkerTasks.Include(t => t.Worker) 
        .Include(t => t.Field).FirstOrDefault(dbTask => dbTask.Id == taskId);
        if (task == null)
        {
            throw new KeyNotFoundException("There is no such task");
        }
        
        return task;
    }

    public IEnumerable<WorkerTask> GetAllWorkerTasks(string searchDate = null)
    {
        IQueryable<WorkerTask> tasksQuery = _context.WorkerTasks
            .Include(t => t.Worker)
            .Include(t => t.Field);

       
        if (!string.IsNullOrEmpty(searchDate))
        {
            // Спроба розібрати дату. Формат "yyyy-MM-dd" найкраще працює з <input type="date">
            if (DateTime.TryParse(searchDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
               
                tasksQuery = tasksQuery.Where(t => t.StartDate.Date == parsedDate.Date);
            }
        }

        
        var tasks = tasksQuery.ToList();

        
        return tasks.Count == 0 ? Enumerable.Empty<WorkerTask>() : tasks;
    }

    public void AddWorkerTask(WorkerTask task)
    {
        if (_context.WorkerTasks.FirstOrDefault(dbTask => dbTask.Id == task.Id) != null) 
            return;
        if (task.RealEndDate<task.StartDate || task.EstimatesEndDate < task.StartDate)
        {
            throw new Exception("Фактична дата завершення або очікувана дата мають бути більше дати початку");
        }
            
        _context.WorkerTasks.Add(task);
        _context.SaveChanges();
        
    }

    public void UpdateWorkerTask(WorkerTask task)
    {
        var existingWorkerTask = _context.WorkerTasks.FirstOrDefault(dbTask => dbTask.Id == task.Id);
        if (existingWorkerTask == null)
        {
            throw new KeyNotFoundException("There is no such task");
        }
        if (task.RealEndDate<task.StartDate || task.EstimatesEndDate < task.StartDate)
        {
            throw new Exception("Фактична дата завершення або очікувана дата мають бути більше дати початку");
        }

        existingWorkerTask.WorkerId = task.WorkerId;
        existingWorkerTask.FieldId = task.FieldId;
        existingWorkerTask.Description = task.Description;
        existingWorkerTask.StartDate = task.StartDate;
        existingWorkerTask.RealEndDate= task.RealEndDate;
        existingWorkerTask.EstimatesEndDate = task.EstimatesEndDate;
        existingWorkerTask.Progress = task.Progress;
        existingWorkerTask.TaskType = task.TaskType;
        
        _context.SaveChanges();
    }

    public void DeleteWorkerTask(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentException("ID must be positive", nameof(taskId));
        }
        
        var task= _context.WorkerTasks.FirstOrDefault(dbTask => dbTask.Id == taskId);
        if (task == null)
        {
            throw new KeyNotFoundException("There is no such task"); 
        }
        
        _context.WorkerTasks.Remove(task);
        _context.SaveChanges();   
    }
    
    public Resource GetResourceById(int resourceId)
    {
        if (resourceId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(resourceId));
        }
        
        var resource = _context.Resources.FirstOrDefault(dbResource => dbResource.Id == resourceId);
        if (resource == null)
        {
            throw new KeyNotFoundException("Resource with such id is not found");
        }
        
        return resource;
    }
    
    public void AddResource(Resource resource)
    {
        if (_context.Resources.FirstOrDefault(dbResource => dbResource.Id == resource.Id) != null) 
            return;
        
        _context.Resources.Add(resource);
        _context.SaveChanges();
    }
    
    public void UpdateResource(Resource resource)
    {
        var existingResource = _context.Resources.FirstOrDefault(dbResource => dbResource.Id == resource.Id);
        if (existingResource == null)
        {
            throw new KeyNotFoundException("Resource is not found");
        }
        
        existingResource.CultureType = resource.CultureType;
        existingResource.SeedPerHectare = resource.SeedPerHectare;
        existingResource.FertilizerPerHectare = resource.FertilizerPerHectare;
        existingResource.WorkerPerHectare = resource.WorkerPerHectare;
        existingResource.WorkerWorkDuralityPerHectare = resource.WorkerWorkDuralityPerHectare;
        existingResource.Yield = resource.Yield;
        existingResource.RequiredMachines = resource.RequiredMachines;
        
        _context.SaveChanges();
    }
    
    public void DeleteResource(int resourceId)
    {
        if (resourceId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(resourceId));
        }
        
        var resource = _context.Resources.FirstOrDefault(dbResource => dbResource.Id == resourceId);
        if (resource == null)
        {
            throw new KeyNotFoundException("Such resource is not found");
        }
        
        _context.Resources.Remove(resource);
        _context.SaveChanges();
    }
    
    public IEnumerable<Resource> GetAllResources()
    {
        var resources = _context.Resources.ToList();
        return resources.Count == 0 ? Enumerable.Empty<Resource>() : resources;
    }
    
    public Warehouse GetWarehouseById(int warehouseId)
    {
        if (warehouseId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(warehouseId));
        }
        
        var warehouse = _context.Warehouses.FirstOrDefault(dbWarehouse => dbWarehouse.Id == warehouseId);
        if (warehouse == null)
        {
            throw new KeyNotFoundException("Warehouse with such id is not found");
        }

        return warehouse;
    }
    
    public IEnumerable<Warehouse> GetAllWarehouses()
    {
        var warehouses = _context.Warehouses.ToList();
        return warehouses.Count == 0 ? Enumerable.Empty<Warehouse>() : warehouses;
    }
    
    public void AddWarehouse(Warehouse warehouse)
    {
        if (_context.Warehouses.FirstOrDefault(dbWarehouse => dbWarehouse.Id == warehouse.Id) != null) 
            return;
        
        _context.Warehouses.Add(warehouse);
        _context.SaveChanges();
    }
    
    public void UpdateWarehouse(Warehouse warehouse)
    {
        var existingWarehouse = _context.Warehouses.FirstOrDefault(dbWarehouse => dbWarehouse.Id == warehouse.Id);
        if (existingWarehouse == null)
        {
            throw new KeyNotFoundException("Warehouse is not found");
        }
        
        existingWarehouse.InventoryItems = warehouse.InventoryItems;
        
        _context.SaveChanges();
    }
    
    public void DeleteWarehouse(int warehouseId)
    {
        if (warehouseId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(warehouseId));
        }
        
        var warehouse = _context.Warehouses.FirstOrDefault(dbWarehouse => dbWarehouse.Id == warehouseId);
        if (warehouse == null)
        {
            throw new KeyNotFoundException("Such warehouse is not found");
        }
        
        _context.Warehouses.Remove(warehouse);
        _context.SaveChanges();
    }
    
    public Resource GetResourceByCultureType(CultureType cultureType)
    {
        if (!Enum.IsDefined(typeof(CultureType), cultureType))
        {
            throw new ArgumentException("Invalid culture type.", nameof(cultureType));
        }
        
        var resource = _context.Resources.FirstOrDefault(resource => resource.CultureType == cultureType);
        if (resource == null)
        {
            throw new KeyNotFoundException($"Resource for culture type {cultureType} not found.");
        }
        
        return resource;
    }
    public Machine GetMachineByMachineType(MachineType machineType)
    {
        if(!Enum.IsDefined(typeof(MachineType), machineType))
        {
            throw new ArgumentException("Invalid machine type.", nameof(machineType));
        }
        
        var machine= _context.Machines.FirstOrDefault(machine => machine.Type == machineType);
        if(machine == null)
        {
            throw new KeyNotFoundException($"Machine for machine type {machineType} not found.");
        }
        
        return machine;
    }
    public IEnumerable<InventoryItem> GetCriticalInventoryItems()
    {
        var items=_context.InventoryItems.ToList();
        return items.Count == 0 ? Enumerable.Empty<InventoryItem>() : items.Where(item => item.Quantity < 5).ToList();
    }

    public IEnumerable<WorkerTask> GetTasksByWorkerId(int workerId)
    {
        if (workerId <= 0)
        {
            throw new ArgumentException("Id must be positive", nameof(workerId));
        }
        var tasksById = _context.WorkerTasks.Where(workerTask => workerTask.Worker.Id == workerId).ToList();

        return tasksById.Count == 0 ? Enumerable.Empty<WorkerTask>() : tasksById;
    }

    public IEnumerable<Machine> GetAvailableMachines()
    {
        var availableMachines= _context.Machines.Where(machine => machine.IsAvailable).ToList();
        return availableMachines.Count == 0 ? Enumerable.Empty<Machine>() : availableMachines;
    }
}