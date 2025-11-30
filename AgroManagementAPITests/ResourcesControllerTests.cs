using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Controllers.v1;
using AgroManagementAPI.DTOs.V1.Resource;
using AgroManagementAPITests.Fixtures;
using AutoMapper;
using AgroManagementAPI.Mappings;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgroManagementAPITests.Controllers
{
    /// <summary>
    /// Tests for ResourcesController - Tests for managing agricultural resources
    /// Runs on all 4 database providers: InMemory, SQLite, PostgreSQL, MySQL, MSSQL
    /// </summary>
    public class ResourcesControllerTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public ResourcesControllerTests()
        {
            _databaseFixture = new DatabaseFixture();
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
            _mapper = mapperConfig.CreateMapper();
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture. DatabaseProvider. Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider. Postgres)]
        [InlineData(DatabaseFixture.DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture.DatabaseProvider. Mssql)]
        public void GetAll_ReturnsOkResult_WithAllResources(DatabaseFixture.DatabaseProvider provider)
        {
            if (! DatabaseFixture. VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var resources = new List<Resource>
            {
                new Resource 
                { 
                    Id = 1, 
                    CultureType = CultureType. Wheat,
                    SeedPerHectare = 150,
                    FertilizerPerHectare = 200,
                    WorkerPerHectare = 2,
                    WorkerWorkDuralityPerHectare = 8,
                    Yield = 5000
                },
                new Resource 
                { 
                    Id = 2, 
                    CultureType = CultureType. Corn,
                    SeedPerHectare = 25,
                    FertilizerPerHectare = 250,
                    WorkerPerHectare = 1.5,
                    WorkerWorkDuralityPerHectare = 6,
                    Yield = 8000
                }
            };

            databaseService.Setup(s => s.GetAllResources()).Returns(resources);

            var controller = new ResourcesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResources = Assert.IsType<List<ResourceResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedResources.Count);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture. DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture. DatabaseProvider.Mssql)]
        public void GetById_ReturnsOkResult_WithCorrectResource(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var resource = new Resource 
            { 
                Id = 1, 
                CultureType = CultureType.Wheat,
                SeedPerHectare = 150,
                FertilizerPerHectare = 200,
                WorkerPerHectare = 2,
                WorkerWorkDuralityPerHectare = 8,
                Yield = 5000
            };

            databaseService.Setup(s => s.GetResourceById(1)).Returns(resource);

            var controller = new ResourcesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResource = Assert.IsType<ResourceResponseDto>(okResult.Value);
            Assert.Equal(CultureType.Wheat, returnedResource.CultureType);
            Assert.Equal(150, returnedResource.SeedPerHectare);
            Assert.Equal(5000, returnedResource.Yield);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        [InlineData(DatabaseFixture.DatabaseProvider.Postgres)]
        [InlineData(DatabaseFixture. DatabaseProvider.MySql)]
        [InlineData(DatabaseFixture. DatabaseProvider.Mssql)]
        public void GetById_ReturnsNotFound_WhenResourceDoesNotExist(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture. VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            databaseService.Setup(s => s.GetResourceById(It.IsAny<int>())).Returns((Resource)null);

            var controller = new ResourcesController(databaseService.Object, _mapper);

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
        public void Create_ReturnsCreatedAtAction_WithNewResource(DatabaseFixture.DatabaseProvider provider)
        {
            if (!DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var createDto = new ResourceCreateDto
            {
                CultureType = CultureType.Wheat,
                SeedPerHectare = 150,
                FertilizerPerHectare = 200,
                WorkerPerHectare = 2,
                WorkerWorkDuralityPerHectare = 8,
                Yield = 5000
            };

            databaseService.Setup(s => s.AddResource(It.IsAny<Resource>()));

            var controller = new ResourcesController(databaseService.Object, _mapper);

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
        public void Delete_ReturnsNoContent_WhenResourceExists(DatabaseFixture.DatabaseProvider provider)
        {
            if (! DatabaseFixture.VerifyDatabaseConnection(provider)) return;

            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();

            var resource = new Resource 
            { 
                Id = 1, 
                CultureType = CultureType.Wheat,
                SeedPerHectare = 150,
                FertilizerPerHectare = 200,
                WorkerPerHectare = 2,
                WorkerWorkDuralityPerHectare = 8,
                Yield = 5000
            };

            databaseService. Setup(s => s.GetResourceById(1)).Returns(resource);
            databaseService.Setup(s => s.DeleteResource(1));

            var controller = new ResourcesController(databaseService.Object, _mapper);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            databaseService.Verify(s => s.DeleteResource(1), Times.Once);
        }
    }
}