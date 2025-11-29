using AgroindustryManagementAPI.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace AgroManagementAPI.Tests.Fixtures
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
                        .UseSqlite("Data Source=:memory:")
                        .Options,

                DatabaseProvider.Postgres =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseNpgsql("Host=localhost;Port=5432;Database=AgroindustryAPIDB_Test;Username=postgres;Password=postgres_password")
                        .Options,

                DatabaseProvider.MySql =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        . UseMySql(
                            "Server=localhost;Port=3306;Database=AgroindustryAPIDB_Test;User Id=root;Password=mysql_password",
                            ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=AgroindustryAPIDB_Test;User Id=root;Password=mysql_password")
                        )
                        .Options,

                DatabaseProvider.Mssql =>
                    new DbContextOptionsBuilder<AGDatabaseContext>()
                        .UseSqlServer("Server=localhost,1433;Database=AgroindustryAPIDB_Test;User Id=sa;Password=YourPassword123! ;TrustServerCertificate=true")
                        .Options,

                _ => throw new ArgumentException("Unsupported provider")
            };

            var context = new AGDatabaseContext(options);
            context.Database.  EnsureCreated();
            return context;
        }
    }
}