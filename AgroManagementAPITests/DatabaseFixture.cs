using AgroindustryManagementAPI.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace AgroManagementAPITests.Fixtures
{
    public class DatabaseFixture
    {
        public enum DatabaseProvider
        {
            InMemory,
            Sqlite,
            Postgres,
            MySql,
            Mssql
        }

        public AGDatabaseContext CreateContext(DatabaseProvider provider = DatabaseProvider.InMemory)
        {
            var options = provider switch
            {
                DatabaseProvider.InMemory =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .Options,

                DatabaseProvider.Sqlite =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseSqlite($"Data Source=:memory:;Cache=Shared")
                        .Options,

                DatabaseProvider.Postgres =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseNpgsql(
                            "Host=localhost;Port=5432;Database=AgroindustryAPIDB_Test;Username=postgres;Password=postgres_password",
                            opts => opts.CommandTimeout(30)
                        )
                        .Options,

                DatabaseProvider.MySql =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseMySql(
                            "Server=localhost;Port=3306;Database=AgroindustryAPIDB_Test;User Id=root;Password=mysql_password;",
                            ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=AgroindustryAPIDB_Test;User Id=root;Password=mysql_password;"),
                            opts => opts.CommandTimeout(30)
                        )
                        .Options,

                DatabaseProvider.Mssql =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseSqlServer(
                            "Server=localhost,1433;Database=AgroindustryAPIDB_Test;User Id=sa;Password=YourPassword123! ;TrustServerCertificate=true;",
                            opts => opts.CommandTimeout(30)
                        )
                        .Options,

                _ => throw new ArgumentException("Unsupported provider: " + provider)
            };

            var context = new AGDatabaseContext(options);

            try
            {
                context.Database.EnsureCreated();

                if (provider != DatabaseProvider.InMemory)
                {
                    ClearDatabase(context, provider);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database for provider {provider}: {ex.Message}", ex);
            }

            return context;
        }

        /// <summary>
        /// Safely clears all data from the database, checking if tables exist first
        /// </summary>
        private void ClearDatabase(AGDatabaseContext context, DatabaseProvider provider)
        {
            try
            {
                // Only attempt to delete if the table exists
                if (context.WorkerTasks.Any() || TableExists(context, "WorkerTasks", provider))
                {
                    context.WorkerTasks.RemoveRange(context.WorkerTasks);
                }

                if (context. InventoryItems.Any() || TableExists(context, "InventoryItems", provider))
                {
                    context.InventoryItems.RemoveRange(context.InventoryItems);
                }

                if (context. Machines.Any() || TableExists(context, "Machines", provider))
                {
                    context. Machines.RemoveRange(context. Machines);
                }

                if (context.Fields.Any() || TableExists(context, "Fields", provider))
                {
                    context.Fields. RemoveRange(context.Fields);
                }

                if (context.Resources.Any() || TableExists(context, "Resources", provider))
                {
                    context.Resources. RemoveRange(context.Resources);
                }

                if (context.Workers.Any() || TableExists(context, "Workers", provider))
                {
                    context.Workers. RemoveRange(context.Workers);
                }

                if (context.Warehouses.Any() || TableExists(context, "Warehouses", provider))
                {
                    context.Warehouses.RemoveRange(context.Warehouses);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Silently ignore - don't crash tests due to cleanup issues
                // This is acceptable for test databases
            }
        }

        /// <summary>
        /// Checks if a table exists in the database
        /// </summary>
        private bool TableExists(AGDatabaseContext context, string tableName, DatabaseProvider provider)
        {
            try
            {
                var connection = context.Database.GetDbConnection();
                var cmd = connection.CreateCommand();

                cmd.CommandText = provider switch
                {
                    DatabaseProvider. Sqlite =>
                        $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';",
                    DatabaseProvider.Postgres =>
                        $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}');",
                    DatabaseProvider.MySql =>
                        $"SELECT EXISTS (SELECT 1 FROM information_schema. TABLES WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{context.Database.GetDbConnection().Database}');",
                    DatabaseProvider. Mssql =>
                        $"SELECT CASE WHEN OBJECT_ID('{tableName}', 'U') IS NOT NULL THEN 1 ELSE 0 END;",
                    _ => ""
                };

                if (connection.State == System.Data.ConnectionState. Closed)
                {
                    connection.Open();
                }

                var result = cmd.ExecuteScalar();
                connection.Close();

                return result != null && (int)result != 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool VerifyDatabaseConnection(DatabaseProvider provider)
        {
            try
            {
                var fixture = new DatabaseFixture();
                var context = fixture.CreateContext(provider);
                var result = context.Database. CanConnect();
                context. Dispose();
                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}