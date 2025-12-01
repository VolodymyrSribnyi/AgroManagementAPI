using AgroindustryManagementAPI.Models;
using AgroManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgroindustryManagementAPI.Services.Database;

public class AGDatabaseContext : DbContext
{
    public DbSet<Field> Fields { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Machine> Machines { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<WorkerTask> WorkerTasks { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public AGDatabaseContext()
    {
    }
    public AGDatabaseContext(DbContextOptions<AGDatabaseContext> options)
        : base(options)
    {
    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseSqlite("Data Source=agroindustry_management.db");
    //    }
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Field>().ToTable("Fields");
        modelBuilder.Entity<Worker>().ToTable("Workers");
        modelBuilder.Entity<Machine>().ToTable("Machines");
        modelBuilder.Entity<InventoryItem>().ToTable("InventoryItems");
        modelBuilder.Entity<WorkerTask>().ToTable("WorkerTasks");
        modelBuilder.Entity<Resource>().ToTable("Resources");
        modelBuilder.Entity<Warehouse>().ToTable("Warehouses");

        modelBuilder.Entity<Field>()
        .Property(f => f.Culture)
        .HasConversion<string>();


        modelBuilder.Entity<Resource>()
            .Property(r => r.CultureType)
            .HasConversion<string>();

        //relationships and constraints 
        modelBuilder.Entity<Field>()
            .HasMany(f => f.Workers)
            .WithMany();

        modelBuilder.Entity<Field>()
            .HasMany(f => f.Machines)
            .WithOne(m => m.Field)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<Field>()
           .HasMany(f => f.Tasks)
           .WithOne(t => t.Field)
           .HasForeignKey(t => t.FieldId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Warehouse>()
            .HasMany(w => w.InventoryItems)
            .WithOne(i => i.Warehouse)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Resource>()
            .HasMany(r => r.RequiredMachines)
            .WithOne(m => m.Resource)
            .HasForeignKey(m => m.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkerTask>()
            .HasOne(t => t.Worker)
            .WithMany(w => w.Tasks)
            .HasForeignKey(t => t.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}