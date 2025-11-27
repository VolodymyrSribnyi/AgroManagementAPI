using AgroindustryManagementAPI.Models;

namespace AgroindustryManagementAPI.Services.Database
{
    public static class DbInitializer
    {
        public static void Initialize(AGDatabaseContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            if (context.Fields.Any())
            {
                return; // DB has been seeded
            }
            var fields = new Field[]
            {
                new Field { Area = 10.5, Culture = CultureType.Wheat,CreatedAt = DateTime.UtcNow,Status = FieldStatus.Fallow },
                new Field { Area = 20.0, Culture = CultureType.Corn,CreatedAt = DateTime.UtcNow, Status = FieldStatus.Fallow},
            };
            context.Fields.AddRange(fields);
            if (context.Resources.Any())
            {
                return; // DB has been seeded
            }
            var resources = new Resource[]
            {
                new Resource {CultureType = CultureType.Wheat,SeedPerHectare = 110,FertilizerPerHectare = 100,WorkerPerHectare = 20,WorkerWorkDuralityPerHectare = 3,Yield = 5000  },
                new Resource { CultureType = CultureType.Corn,SeedPerHectare = 25,FertilizerPerHectare = 60,WorkerPerHectare = 10,WorkerWorkDuralityPerHectare = 2,Yield = 80000 },
            };
            context.Resources.AddRange(resources);
            if (context.Workers.Any())
            {
                return; // DB has been seeded
            }
            var workers = new Worker[]
            {
                new Worker{ FirstName = "John", LastName = "Doe", Age=35, HourlyRate=100, HoursWorked=80, IsActive=false },
                new Worker{ FirstName = "Jane", LastName = "Smith", Age=28, HourlyRate=120, HoursWorked=60, IsActive=true },
            };
            context.Workers.AddRange(workers);
            context.SaveChanges();
        }
    }
}
