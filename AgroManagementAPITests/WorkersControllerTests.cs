using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Controllers.v1;
using AgroManagementAPI.DTOs.V1.Worker;
using AgroManagementAPITests.Fixtures;
using AutoMapper;
using AgroManagementAPI.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AgroManagementAPITests.Controllers
{
    public class WorkersControllerTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public WorkersControllerTests()
        {
            _databaseFixture = new DatabaseFixture();
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()), new LoggerFactory());
            _mapper = mapperConfig.CreateMapper();
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider.Mssql)]
        public void GetAll_ReturnsOkResult_WithAllWorkers(DatabaseFixture.DatabaseProvider provider)
        {
            // Skip test if database is not available
            if (!DatabaseFixture.VerifyDatabaseConnection(provider))
            {
                return; // Skip this test
            }

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var workers = new List<Worker>
            {
                new Worker { Id = 1, FirstName = "John", LastName = "Doe", Age = 30, HourlyRate = 15m, IsActive = true },
                new Worker { Id = 2, FirstName = "Jane", LastName = "Smith", Age = 25, HourlyRate = 14m, IsActive = true }
            };

            databaseService.Setup(s => s.GetAllWorkers()). Returns(workers);

            var controller = new WorkersController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWorkers = Assert.IsType<List<WorkerResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedWorkers.Count);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture. DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture. DatabaseProvider.Mssql)]
        public void GetById_ReturnsOkResult_WithCorrectWorker(DatabaseFixture. DatabaseProvider provider)
        {
            // Skip test if database is not available
            if (!DatabaseFixture.VerifyDatabaseConnection(provider))
            {
                return; // Skip this test
            }

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var worker = new Worker
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                HourlyRate = 15m,
                IsActive = true
            };

            databaseService.Setup(s => s.GetWorkerById(1)).Returns(worker);

            var controller = new WorkersController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWorker = Assert. IsType<WorkerResponseDto>(okResult.Value);
            Assert.Equal("John", returnedWorker.FirstName);
            Assert.Equal("Doe", returnedWorker. LastName);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider.Mssql)]
        public void GetById_ReturnsNotFound_WhenWorkerDoesNotExist(DatabaseFixture.DatabaseProvider provider)
        {
            // Skip test if database is not available
            if (!DatabaseFixture.VerifyDatabaseConnection(provider))
            {
                return; // Skip this test
            }

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            databaseService.Setup(s => s.GetWorkerById(It.IsAny<int>())).Returns((Worker)null);

            var controller = new WorkersController(databaseService.Object, _mapper);

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
        public void Create_ReturnsCreatedAtAction_WithNewWorker(DatabaseFixture.DatabaseProvider provider)
        {
            // Skip test if database is not available
            if (!DatabaseFixture. VerifyDatabaseConnection(provider))
            {
                return; // Skip this test
            }

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var createDto = new WorkerCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                HourlyRate = 15m,
                IsActive = true
            };

            var newWorker = new Worker
            {
                Id = 1,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Age = createDto.Age,
                HourlyRate = createDto.HourlyRate,
                IsActive = createDto. IsActive
            };

            databaseService.Setup(s => s.AddWorker(It.IsAny<Worker>()));

            var controller = new WorkersController(databaseService.Object, _mapper);

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
        public void Delete_ReturnsNoContent_WhenWorkerExists(DatabaseFixture.DatabaseProvider provider)
        {
            // Skip test if database is not available
            if (!DatabaseFixture.VerifyDatabaseConnection(provider))
            {
                return; // Skip this test
            }

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var worker = new Worker
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Age = 30,
                HourlyRate = 15m,
                IsActive = true
            };

            databaseService.Setup(s => s.GetWorkerById(1)).Returns(worker);
            databaseService.Setup(s => s.DeleteWorker(1));

            var controller = new WorkersController(databaseService.Object, _mapper);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            databaseService.Verify(s => s.DeleteWorker(1), Times.Once);
        }
    }
}