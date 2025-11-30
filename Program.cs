using AgroindustryManagementAPI.Services.Calculations;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Mappings;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace AgroManagementAPI
{
    /// <summary>
    /// Main program class for configuring and running the Agroindustry Management API application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="Exception"></exception>
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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173") // React app URL
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Agroindustry Management API V1",
                    Version = "v1",
                    Description = "RESTful API for managing agroindustry operations",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@agromanagement.com"
                    }
                });
                
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Agroindustry Management API V2",
                    Version = "v2",
                    Description = "RESTful API V2 with filtering and pagination for managing agroindustry operations",
                    Contact = new OpenApiContact
                    {
                        Name = "API Support",
                        Email = "support@agromanagement.com"
                    }
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

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
                    case "mysql":
                        options.UseMySql(connectionStrings.GetValue<string>("MySQL"), 
                            ServerVersion.AutoDetect(connectionStrings.GetValue<string>("MySQL")));
                        break;
                    case "inmemory":
                        options.UseInMemoryDatabase("AGInMemoryDb");
                        break;
                    default:
                        throw new Exception("Unsupported database provider: " + dbProvider);
                }
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AGDatabaseContext>();
                dbContext.Database.EnsureCreated();
            }
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Agroindustry Management API V1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Agroindustry Management API V2");
                    c.DocumentTitle = "Agroindustry Management API - Documentation";
                });
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}