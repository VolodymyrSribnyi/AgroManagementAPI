using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Area = table.Column<double>(type: "REAL", nullable: false),
                    Culture = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CultureType = table.Column<string>(type: "TEXT", nullable: false),
                    SeedPerHectare = table.Column<double>(type: "REAL", nullable: false),
                    FertilizerPerHectare = table.Column<double>(type: "REAL", nullable: false),
                    WorkerPerHectare = table.Column<double>(type: "REAL", nullable: false),
                    WorkerWorkDuralityPerHectare = table.Column<double>(type: "REAL", nullable: false),
                    Yield = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    FuelConsumption = table.Column<double>(type: "REAL", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    WorkDuralityPerHectare = table.Column<double>(type: "REAL", nullable: false),
                    FieldId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Machines_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Machines_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldWorker",
                columns: table => new
                {
                    FieldId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldWorker", x => new { x.FieldId, x.WorkersId });
                    table.ForeignKey(
                        name: "FK_FieldWorker_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldWorker_Workers_WorkersId",
                        column: x => x.WorkersId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskType = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<double>(type: "REAL", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RealEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatesEndDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerTasks_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkerTasks_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldWorker_WorkersId",
                table: "FieldWorker",
                column: "WorkersId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_WarehouseId",
                table: "InventoryItems",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_FieldId",
                table: "Machines",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_ResourceId",
                table: "Machines",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerTasks_FieldId",
                table: "WorkerTasks",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerTasks_WorkerId",
                table: "WorkerTasks",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldWorker");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "WorkerTasks");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "Workers");
        }
    }
}
