using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services. Database;
using AgroManagementAPI.Controllers. v1;
using AgroManagementAPI.DTOs.V1.InventoryItem;
using AgroManagementAPI.DTOs.V1. Warehouse;
using AgroManagementAPITests.Fixtures;
using AutoMapper;
using AgroManagementAPI.Mappings;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgroManagementAPITests.Controllers
{
    /// <summary>
    /// Tests for InventoryItemsController - Tests for managing inventory items in warehouses
    /// Runs on all 4 database providers: InMemory, SQLite, PostgreSQL, MySQL, MSSQL
    /// </summary>
    public class InventoryItemsControllerTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public InventoryItemsControllerTests()
        {
            _databaseFixture = new DatabaseFixture();
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()), new LoggerFactory());
            _mapper = mapperConfig.CreateMapper();
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider. Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider.Mssql)]
        public void GetAll_ReturnsOkResult_WithAllInventoryItems(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var items = new List<InventoryItem>
            {
                new InventoryItem { Id = 1, Name = "Wheat", Quantity = 500, Unit = "kg", Warehouse = new Warehouse { Id = 1 } },
                new InventoryItem { Id = 2, Name = "Corn", Quantity = 300, Unit = "kg", Warehouse = new Warehouse { Id = 2 } },
                new InventoryItem { Id = 3, Name = "Fertilizer", Quantity = 200, Unit = "kg", Warehouse = new Warehouse { Id = 3 } }
            };

            databaseService.Setup(s => s.GetAllInventoryItems()).Returns(items);

            var controller = new InventoryItemsController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItems = Assert. IsType<List<InventoryItemResponseDto>>(okResult.Value);
            Assert.Equal(3, returnedItems.Count);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider. MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void GetById_ReturnsOkResult_WithCorrectInventoryItem(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var item = new InventoryItem 
            { 
                Id = 1, 
                Name = "Wheat", 
                Quantity = 500, 
                Unit = "kg", 
                Warehouse = new Warehouse { Id = 1 }
            };

            databaseService.Setup(s => s. GetInventoryItemById(1)). Returns(item);

            var controller = new InventoryItemsController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItem = Assert.IsType<InventoryItemResponseDto>(okResult.Value);
            Assert.Equal("Wheat", returnedItem.Name);
            Assert.Equal(500, returnedItem.Quantity);
            Assert.Equal("kg", returnedItem.Unit);
            Assert.Equal(1, returnedItem. WarehouseId);
        }

        [Theory]
        [InlineData(DatabaseFixture. DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void GetById_ReturnsNotFound_WhenInventoryItemDoesNotExist(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture. VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            databaseService.Setup(s => s.GetInventoryItemById(It.IsAny<int>())).Returns((InventoryItem)null);

            var controller = new InventoryItemsController(databaseService.Object, _mapper);

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
        public void Create_ReturnsCreatedAtAction_WithNewInventoryItem(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var createDto = new InventoryItemCreateDto
            {
                Name = "Wheat",
                Quantity = 500,
                Unit = "kg",
                WarehouseId = 1
            };

            databaseService.Setup(s => s. AddInventoryItem(It.IsAny<InventoryItem>()));

            var controller = new InventoryItemsController(databaseService.Object, _mapper);

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
        public void Delete_ReturnsNoContent_WhenInventoryItemExists(DatabaseFixture.DatabaseProvider provider)
        {
            if (! DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var item = new InventoryItem 
            { 
                Id = 1, 
                Name = "Wheat", 
                Quantity = 500, 
                Unit = "kg", 
                Warehouse = new Warehouse { Id = 1 }
            };

            databaseService.Setup(s => s. GetInventoryItemById(1)).Returns(item);
            databaseService.Setup(s => s.DeleteInventoryItem(1));

            var controller = new InventoryItemsController(databaseService.Object, _mapper);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            databaseService.Verify(s => s.DeleteInventoryItem(1), Times.Once);
        }
    }
}