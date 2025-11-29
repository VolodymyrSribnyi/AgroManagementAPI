using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Controllers.v1;
using AgroManagementAPI.DTOs.V1.Warehouse;
using AgroManagementAPITests.Fixtures;
using AutoMapper;
using AgroManagementAPI.Mappings;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgroManagementAPITests.Controllers
{
    /// <summary>
    /// Tests for WarehousesController - Tests for managing warehouses and storage facilities
    /// Runs on all 4 database providers: InMemory, SQLite, PostgreSQL, MySQL, Mssql
    /// </summary>
    public class WarehousesControllerTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public WarehousesControllerTests()
        {
            _databaseFixture = new DatabaseFixture();
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()), new LoggerFactory());
            _mapper = mapperConfig.CreateMapper();
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider. MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void GetAll_ReturnsOkResult_WithAllWarehouses(DatabaseFixture. DatabaseProvider provider)
        {
            if (!DatabaseFixture. VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var warehouses = new List<Warehouse>
            {
                new Warehouse { Id = 1 },
                new Warehouse { Id = 2 }
            };

            databaseService.Setup(s => s.GetAllWarehouses()).Returns(warehouses);

            var controller = new WarehousesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWarehouses = Assert.IsType<List<WarehouseResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedWarehouses.Count);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider. Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider.Mssql)]
        public void GetById_ReturnsOkResult_WithCorrectWarehouse(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var warehouse = new Warehouse { Id = 1 };

            databaseService.Setup(s => s.GetWarehouseById(1)).Returns(warehouse);

            var controller = new WarehousesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWarehouse = Assert. IsType<WarehouseResponseDto>(okResult.Value);
            Assert.Equal(1, returnedWarehouse.Id);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider. MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void GetById_ReturnsNotFound_WhenWarehouseDoesNotExist(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture. VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            databaseService.Setup(s => s.GetWarehouseById(It.IsAny<int>())).Returns((Warehouse)null);

            var controller = new WarehousesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider.Mssql)]
        public void Create_ReturnsCreatedAtAction_WithNewWarehouse(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var createDto = new WarehouseCreateDto();

            databaseService.Setup(s => s.AddWarehouse(It.IsAny<Warehouse>()));

            var controller = new WarehousesController(databaseService.Object, _mapper);

            // Act
            var result = controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetById), createdResult. ActionName);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider. MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void Delete_ReturnsNoContent_WhenWarehouseExists(DatabaseFixture.DatabaseProvider provider)
        {
            if (! DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var warehouse = new Warehouse { Id = 1 };

            databaseService.Setup(s => s.GetWarehouseById(1)).Returns(warehouse);
            databaseService.Setup(s => s.DeleteWarehouse(1));

            var controller = new WarehousesController(databaseService.Object, _mapper);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            databaseService.Verify(s => s.DeleteWarehouse(1), Times.Once);
        }
    }
}