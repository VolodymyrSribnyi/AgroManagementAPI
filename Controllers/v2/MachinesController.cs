using AutoMapper;
using AgroindustryManagementAPI. Models;
using AgroindustryManagementAPI.Services. Database;
using AgroManagementAPI.DTOs.V1.Machine;
using AgroManagementAPI.DTOs.V2.Machine;
using AgroManagementAPI.DTOs. V2.Common;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public MachinesController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all machines (V1 compatibility)
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var machines = _databaseService.GetAllMachines();
                var machinesDto = _mapper.Map<List<MachineResponseDto>>(machines);
                return Ok(machinesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving machines: " + ex.Message });
            }
        }

        /// <summary>
        /// Get machines with filtering and pagination (V2 NEW)
        /// </summary>
        [HttpGet("filter")]
        public IActionResult GetWithFilter([FromQuery] MachineFilterDto filterDto)
        {
            try
            {
                var machines = _databaseService.GetAllMachines();

                // Apply filters
                if (filterDto.Type.HasValue)
                    machines = machines.Where(m => m.Type == filterDto.Type. Value).ToList();

                if (filterDto.IsAvailable.HasValue)
                    machines = machines.Where(m => m.IsAvailable == filterDto.IsAvailable.Value).ToList();

                if (filterDto.MinFuelConsumption.HasValue)
                    machines = machines.Where(m => m.FuelConsumption >= filterDto.MinFuelConsumption.Value).ToList();

                if (filterDto. MaxFuelConsumption.HasValue)
                    machines = machines.Where(m => m.FuelConsumption <= filterDto. MaxFuelConsumption.Value).ToList();

                // Pagination
                int pageNumber = filterDto.PageNumber ?? 1;
                int pageSize = filterDto.PageSize ?? 10;

                int totalCount = machines.Count();
                var paginatedMachines = machines.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var machinesDto = _mapper.Map<List<MachineResponseDto>>(paginatedMachines);

                var result = new PaginatedResultDto<MachineResponseDto>
                {
                    Items = machinesDto,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving machines: " + ex.Message });
            }
        }

        /// <summary>
        /// Get machine by ID (V1 compatibility)
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var machine = _databaseService.GetMachineById(id);
                if (machine == null)
                    return NotFound(new { message = "Machine not found" });

                var machineDto = _mapper. Map<MachineResponseDto>(machine);
                return Ok(machineDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving machine: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new machine (V1 compatibility)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] MachineCreateDto machineCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var machine = _mapper.Map<Machine>(machineCreateDto);
                _databaseService.AddMachine(machine);
                var createdMachineDto = _mapper.Map<MachineResponseDto>(machine);
                return CreatedAtAction(nameof(GetById), new { id = machine.Id }, createdMachineDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating machine: " + ex. Message });
            }
        }

        /// <summary>
        /// Update an existing machine (V1 compatibility)
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MachineUpdateDto machineUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingMachine = _databaseService.GetMachineById(id);
                if (existingMachine == null)
                    return NotFound(new { message = "Machine not found" });

                _mapper.Map(machineUpdateDto, existingMachine);
                _databaseService.UpdateMachine(existingMachine);
                var updatedMachineDto = _mapper.Map<MachineResponseDto>(existingMachine);
                return Ok(updatedMachineDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating machine: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a machine (V1 compatibility)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var machine = _databaseService.GetMachineById(id);
                if (machine == null)
                    return NotFound(new { message = "Machine not found" });

                _databaseService.DeleteMachine(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting machine: " + ex. Message });
            }
        }
    }
}