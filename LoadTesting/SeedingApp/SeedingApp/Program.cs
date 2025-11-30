using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SeedingApp;

// Enums matching the main API
public enum CultureType { Wheat, Corn, Soybean, Rice, Cotton }
public enum FieldStatus { Planted, Harvested, Fallow }
public enum MachineType { Tractor, Harvester, Plow, Seeder, Sprayer }
public enum TaskType { Planting, Harvesting, Fertilizing, Irrigating, Spraying }

// Models matching the main API
public class Field
{
    public int Id { get; set; }
    public double Area { get; set; }
    public CultureType Culture { get; set; }
    public FieldStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Worker
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public decimal HourlyRate { get; set; }
    public bool IsActive { get; set; }
    public decimal HoursWorked { get; set; }
}

public class Resource
{
    public int Id { get; set; }
    public CultureType CultureType { get; set; }
    public double SeedPerHectare { get; set; }
    public double FertilizerPerHectare { get; set; }
    public double WorkerPerHectare { get; set; }
    public double WorkerWorkDuralityPerHectare { get; set; }
    public double Yield { get; set; }
}

public class Machine
{
    public int Id { get; set; }
    public MachineType Type { get; set; }
    public double FuelConsumption { get; set; }
    public bool IsAvailable { get; set; }
    public double WorkDuralityPerHectare { get; set; }
    public int? FieldId { get; set; }
    public int ResourceId { get; set; }
}

public class Warehouse
{
    public int Id { get; set; }
}

public class InventoryItem
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

// DbContext for seeding
public class SeedingDbContext : DbContext
{
    private readonly string _connectionString;

    public SeedingDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Field> Fields { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Field>().ToTable("Fields");
        modelBuilder.Entity<Worker>().ToTable("Workers");
        modelBuilder.Entity<Resource>().ToTable("Resources");
        modelBuilder.Entity<Machine>().ToTable("Machines");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");
        modelBuilder.Entity<InventoryItem>().ToTable("InventoryItems");

        modelBuilder.Entity<Field>()
            .Property(f => f.Culture)
            .HasConversion<string>();

        modelBuilder.Entity<Resource>()
            .Property(r => r.CultureType)
            .HasConversion<string>();
    }
}

class Program
{
    static void Main(string[] args)
    {
        var connectionString = args.Length > 0 ? args[0] : "Data Source=../../../AgroindustryAPI.db";
        
        Console.WriteLine("===========================================");
        Console.WriteLine("  AgroManagement API Database Seeding Tool");
        Console.WriteLine("===========================================");
        Console.WriteLine($"Connection String: {connectionString}");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        using var context = new SeedingDbContext(connectionString);
        
        // Ensure database exists
        context.Database.EnsureCreated();

        // Configure Faker random seed for reproducibility
        Randomizer.Seed = new Random(42);

        // Clear existing data
        Console.WriteLine("Clearing existing data...");
        ClearExistingData(context);

        // Seed data
        Console.WriteLine("Seeding Fields...");
        var fields = SeedFields(context, 2000);
        Console.WriteLine($"  Created {fields.Count} fields");

        Console.WriteLine("Seeding Workers...");
        var workers = SeedWorkers(context, 2000);
        Console.WriteLine($"  Created {workers.Count} workers");

        Console.WriteLine("Seeding Resources...");
        var resources = SeedResources(context, 2000);
        Console.WriteLine($"  Created {resources.Count} resources");

        Console.WriteLine("Seeding Machines...");
        var machines = SeedMachines(context, resources, fields, 2000);
        Console.WriteLine($"  Created {machines.Count} machines");

        Console.WriteLine("Seeding Warehouses...");
        var warehouses = SeedWarehouses(context, 500);
        Console.WriteLine($"  Created {warehouses.Count} warehouses");

        Console.WriteLine("Seeding InventoryItems...");
        var inventoryItems = SeedInventoryItems(context, warehouses, 4000);
        Console.WriteLine($"  Created {inventoryItems.Count} inventory items");

        stopwatch.Stop();

        var totalRecords = fields.Count + workers.Count + resources.Count + 
                          machines.Count + warehouses.Count + inventoryItems.Count;

        Console.WriteLine();
        Console.WriteLine("===========================================");
        Console.WriteLine("  Seeding Complete!");
        Console.WriteLine("===========================================");
        Console.WriteLine($"Total records created: {totalRecords}");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        Console.WriteLine();
        Console.WriteLine("Summary:");
        Console.WriteLine($"  - Fields: {fields.Count}");
        Console.WriteLine($"  - Workers: {workers.Count}");
        Console.WriteLine($"  - Resources: {resources.Count}");
        Console.WriteLine($"  - Machines: {machines.Count}");
        Console.WriteLine($"  - Warehouses: {warehouses.Count}");
        Console.WriteLine($"  - InventoryItems: {inventoryItems.Count}");
    }

    static void ClearExistingData(SeedingDbContext context)
    {
        // Clear in order respecting foreign keys
        context.Database.ExecuteSqlRaw("DELETE FROM InventoryItems");
        context.Database.ExecuteSqlRaw("DELETE FROM Machines");
        context.Database.ExecuteSqlRaw("DELETE FROM Warehouses");
        context.Database.ExecuteSqlRaw("DELETE FROM Resources");
        context.Database.ExecuteSqlRaw("DELETE FROM Workers");
        context.Database.ExecuteSqlRaw("DELETE FROM Fields");
    }

    static List<Field> SeedFields(SeedingDbContext context, int count)
    {
        var faker = new Faker<Field>()
            .RuleFor(f => f.Area, f => f.Random.Double(10, 1000))
            .RuleFor(f => f.Culture, f => f.PickRandom<CultureType>())
            .RuleFor(f => f.Status, f => f.PickRandom<FieldStatus>())
            .RuleFor(f => f.CreatedAt, f => f.Date.Past(5));

        var fields = faker.Generate(count);
        context.Fields.AddRange(fields);
        context.SaveChanges();
        return fields;
    }

    static List<Worker> SeedWorkers(SeedingDbContext context, int count)
    {
        var faker = new Faker<Worker>()
            .RuleFor(w => w.FirstName, f => f.Name.FirstName())
            .RuleFor(w => w.LastName, f => f.Name.LastName())
            .RuleFor(w => w.Age, f => f.Random.Int(18, 65))
            .RuleFor(w => w.HourlyRate, f => f.Random.Decimal(10, 50))
            .RuleFor(w => w.IsActive, f => f.Random.Bool(0.8f))
            .RuleFor(w => w.HoursWorked, f => f.Random.Decimal(0, 2000));

        var workers = faker.Generate(count);
        context.Workers.AddRange(workers);
        context.SaveChanges();
        return workers;
    }

    static List<Resource> SeedResources(SeedingDbContext context, int count)
    {
        var faker = new Faker<Resource>()
            .RuleFor(r => r.CultureType, f => f.PickRandom<CultureType>())
            .RuleFor(r => r.SeedPerHectare, f => f.Random.Double(50, 200))
            .RuleFor(r => r.FertilizerPerHectare, f => f.Random.Double(100, 500))
            .RuleFor(r => r.WorkerPerHectare, f => f.Random.Double(0.5, 5))
            .RuleFor(r => r.WorkerWorkDuralityPerHectare, f => f.Random.Double(1, 10))
            .RuleFor(r => r.Yield, f => f.Random.Double(2, 15));

        var resources = faker.Generate(count);
        context.Resources.AddRange(resources);
        context.SaveChanges();
        return resources;
    }

    static List<Machine> SeedMachines(SeedingDbContext context, List<Resource> resources, List<Field> fields, int count)
    {
        var faker = new Faker<Machine>()
            .RuleFor(m => m.Type, f => f.PickRandom<MachineType>())
            .RuleFor(m => m.FuelConsumption, f => f.Random.Double(5, 50))
            .RuleFor(m => m.IsAvailable, f => f.Random.Bool(0.7f))
            .RuleFor(m => m.WorkDuralityPerHectare, f => f.Random.Double(0.5, 3))
            .RuleFor(m => m.FieldId, f => f.Random.Bool(0.5f) ? f.PickRandom(fields).Id : null)
            .RuleFor(m => m.ResourceId, f => f.PickRandom(resources).Id);

        var machines = faker.Generate(count);
        context.Machines.AddRange(machines);
        context.SaveChanges();
        return machines;
    }

    static List<Warehouse> SeedWarehouses(SeedingDbContext context, int count)
    {
        var warehouses = Enumerable.Range(1, count)
            .Select(_ => new Warehouse())
            .ToList();

        context.Warehouses.AddRange(warehouses);
        context.SaveChanges();
        return warehouses;
    }

    static List<InventoryItem> SeedInventoryItems(SeedingDbContext context, List<Warehouse> warehouses, int count)
    {
        var inventoryNames = new[]
        {
            "Wheat Seeds", "Corn Seeds", "Soybean Seeds", "Rice Seeds", "Cotton Seeds",
            "Nitrogen Fertilizer", "Phosphorus Fertilizer", "Potassium Fertilizer",
            "Herbicide", "Pesticide", "Fungicide", "Diesel Fuel", "Gasoline",
            "Engine Oil", "Hydraulic Fluid", "Spare Parts", "Tires", "Filters",
            "Irrigation Pipes", "Sprinkler Heads", "Water Pumps", "Tools",
            "Safety Equipment", "Packaging Materials", "Harvest Bags"
        };

        var units = new[] { "kg", "L", "units", "tons", "bags", "boxes", "pieces" };

        var faker = new Faker<InventoryItem>()
            .RuleFor(i => i.WarehouseId, f => f.PickRandom(warehouses).Id)
            .RuleFor(i => i.Name, f => f.PickRandom(inventoryNames))
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10000))
            .RuleFor(i => i.Unit, f => f.PickRandom(units));

        var items = faker.Generate(count);
        context.InventoryItems.AddRange(items);
        context.SaveChanges();
        return items;
    }
}
