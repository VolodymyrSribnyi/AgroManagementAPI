using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.Machine;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing agricultural machines and equipment
    /// </summary>
    [ApiVersion("1.0")]
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
        /// Get all machines
        /// </summary>
        /// <returns>List of all machines</returns>
        /// <response code="200">Machines retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<MachineResponseDto>), StatusCodes. Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// Get a machine by ID
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <returns>Machine data</returns>
        /// <response code="200">Machine retrieved successfully</response>
        /// <response code="404">Machine not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MachineResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes. Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var machine = _databaseService.GetMachineById(id);
                if (machine == null)
                    return NotFound(new { message = "Machine not found" });

                var machineDto = _mapper.Map<MachineResponseDto>(machine);
                return Ok(machineDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving machine: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new machine
        /// </summary>
        /// <param name="machineCreateDto">Machine creation data</param>
        /// <returns>Created machine</returns>
        /// <response code="201">Machine created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(MachineResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes. Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] MachineCreateDto machineCreateDto)
        {
            if (!ModelState. IsValid)
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
                return StatusCode(500, new { message = "Error creating machine: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing machine
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <param name="machineUpdateDto">Updated machine data</param>
        /// <returns>Updated machine</returns>
        /// <response code="200">Machine updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Machine not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MachineResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] MachineUpdateDto machineUpdateDto)
        {
            if (!ModelState. IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingMachine = _databaseService.GetMachineById(id);
                if (existingMachine == null)
                    return NotFound(new { message = "Machine not found" });

                _mapper. Map(machineUpdateDto, existingMachine);
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
        /// Delete a machine
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <response code="204">Machine deleted successfully</response>
        /// <response code="404">Machine not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                return StatusCode(500, new { message = "Error deleting machine: " + ex.Message });
            }
        }
    }
}