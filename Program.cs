
using AgroindustryManagementAPI.Services.Calculations;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Mappings;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AgroManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var dbProvider = builder.Configuration["DatabaseSettings:Provider"];
            var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";             // v1, v2
                options.SubstituteApiVersionInUrl = true;     // replaces {version} in route
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Agroindustry Management API V1",
                    Version = "v1"
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Agroindustry Management API V2",
                    Version = "v2"
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
            
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            
            builder.Services.AddScoped<IAGCalculationService, AGCalculationService>();
            builder.Services.AddScoped<IAGDatabaseService, AGDatabaseService>();
            builder.Services.AddDbContext<AGDatabaseContext>(options =>
            {
                switch (dbProvider?.ToLower())
                {
                    case "sqlite":
                        options.UseSqlite(connectionStrings.GetValue<string>("Sqlite"));
                        break;
                    case "mssql":
                        options.UseSqlServer(connectionStrings.GetValue<string>("MSSQL"));
                        break;
                    case "postgres":
                        options.UseNpgsql(connectionStrings.GetValue<string>("Postgres"));
                        break;
                    case "inmemory":
                        options.UseInMemoryDatabase("AGInMemoryDb");
                        break;
                    default:
                        throw new Exception("Unsupported database provider: " + dbProvider);
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c=>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agroindustry Management API V1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Agroindustry Management API V2");
                });
                app.UseExceptionHandler("/error");
            }
            ;

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
