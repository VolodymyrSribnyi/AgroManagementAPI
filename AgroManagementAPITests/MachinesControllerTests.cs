using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.Controllers.v1;
using AgroManagementAPI.DTOs.V1. Machine;
using AgroManagementAPI.Tests.Fixtures;
using AutoMapper;
using AgroManagementAPI.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AgroManagementAPI.Tests.Controllers
{
    public class MachinesControllerTests
    {
        private readonly DatabaseFixture _databaseFixture;
        private readonly IMapper _mapper;

        public MachinesControllerTests()
        {
            _databaseFixture = new DatabaseFixture();
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()), new LoggerFactory());
            _mapper = mapperConfig.CreateMapper();
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        public void GetAll_ReturnsOkResult_WithAllMachines(DatabaseFixture.DatabaseProvider provider)
        {
            // Arrange
            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();
            
            var machines = new List<Machine>
            {
                new Machine 
                { 
                    Id = 1, 
                    Type = MachineType.Tractor, 
                    FuelConsumption = 10.5, 
                    IsAvailable = true, 
                    WorkDuralityPerHectare = 2.5,
                    ResourceId = 1
                },
                new Machine 
                { 
                    Id = 2, 
                    Type = MachineType.Harvester, 
                    FuelConsumption = 15.0, 
                    IsAvailable = false, 
                    WorkDuralityPerHectare = 3.0,
                    ResourceId = 1
                }
            };
            
            databaseService.Setup(s => s.GetAllMachines()).Returns(machines);
            
            var controller = new MachinesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetAll();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMachines = Assert.IsType<List<MachineResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedMachines.Count);
        }

        [Theory]
        [InlineData(DatabaseFixture. DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        public void GetById_ReturnsOkResult_WithCorrectMachine(DatabaseFixture. DatabaseProvider provider)
        {
            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();
            
            var machine = new Machine 
            { 
                Id = 1, 
                Type = MachineType.Tractor, 
                FuelConsumption = 10.5, 
                IsAvailable = true, 
                WorkDuralityPerHectare = 2.5,
                ResourceId = 1
            };
            
            databaseService.Setup(s => s. GetMachineById(1)). Returns(machine);
            
            var controller = new MachinesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMachine = Assert.IsType<MachineResponseDto>(okResult.Value);
            Assert.Equal(MachineType.Tractor, returnedMachine.Type);
            Assert.True(returnedMachine.IsAvailable);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider. InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        public void GetById_ReturnsNotFound_WhenMachineDoesNotExist(DatabaseFixture.DatabaseProvider provider)
        {
            // Arrange
            var context = _databaseFixture. CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();
            
            databaseService.Setup(s => s.GetMachineById(It.IsAny<int>())).Returns((Machine)null);
            
            var controller = new MachinesController(databaseService.Object, _mapper);

            // Act
            var result = controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        public void Create_ReturnsCreatedAtAction_WithNewMachine(DatabaseFixture.DatabaseProvider provider)
        {
            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();
            
            var createDto = new MachineCreateDto 
            { 
                Type = MachineType.Tractor, 
                FuelConsumption = 10.5, 
                IsAvailable = true, 
                WorkDuralityPerHectare = 2.5,
                ResourceId = 1
            };
            
            var newMachine = new Machine 
            { 
                Id = 1,
                Type = createDto.Type, 
                FuelConsumption = createDto.FuelConsumption, 
                IsAvailable = createDto.IsAvailable, 
                WorkDuralityPerHectare = createDto. WorkDuralityPerHectare,
                ResourceId = createDto.ResourceId
            };
            
            databaseService.Setup(s => s.AddMachine(It.IsAny<Machine>()));
            
            var controller = new MachinesController(databaseService.Object, _mapper);

            // Act
            var result = controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(controller.GetById), createdResult. ActionName);
        }

        [Theory]
        [InlineData(DatabaseFixture.DatabaseProvider.InMemory)]
        [InlineData(DatabaseFixture.DatabaseProvider.Sqlite)]
        public void Delete_ReturnsNoContent_WhenMachineExists(DatabaseFixture.DatabaseProvider provider)
        {
            // Arrange
            var context = _databaseFixture.CreateContext(provider);
            var databaseService = new Mock<IAGDatabaseService>();
            
            var machine = new Machine 
            { 
                Id = 1, 
                Type = MachineType. Tractor, 
                FuelConsumption = 10.5, 
                IsAvailable = true, 
                WorkDuralityPerHectare = 2.5,
                ResourceId = 1
            };
            
            databaseService.Setup(s => s.GetMachineById(1)).Returns(machine);
            databaseService.Setup(s => s.DeleteMachine(1));
            
            var controller = new MachinesController(databaseService. Object, _mapper);

            // Act
            var result = controller. Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            databaseService.Verify(s => s.DeleteMachine(1), Times.Once);
        }
    }
}