using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.Worker;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing workers
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public WorkersController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all workers
        /// </summary>
        /// <returns>List of all workers</returns>
        /// <response code="200">List of workers successfully retrieved</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<WorkerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var workers = _databaseService.GetAllWorkers();
                var workersDto = _mapper.Map<List<WorkerResponseDto>>(workers);
                return Ok(workersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving workers: " + ex.Message });
            }
        }

        /// <summary>
        /// Get a worker by ID
        /// </summary>
        /// <param name="id">Worker ID</param>
        /// <returns>Worker data</returns>
        /// <response code="200">Worker successfully retrieved</response>
        /// <response code="404">Worker not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var worker = _databaseService.GetWorkerById(id);
                if (worker == null)
                    return NotFound(new { message = "Worker not found" });

                var workerDto = _mapper.Map<WorkerResponseDto>(worker);
                return Ok(workerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new worker
        /// </summary>
        /// <param name="workerCreateDto">Worker data</param>
        /// <returns>Created worker</returns>
        /// <response code="201">Worker successfully created</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] WorkerCreateDto workerCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var worker = _mapper.Map<Worker>(workerCreateDto);
                _databaseService.AddWorker(worker);
                var createdWorkerDto = _mapper.Map<WorkerResponseDto>(worker);
                return CreatedAtAction(nameof(GetById), new { id = worker.Id }, createdWorkerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating worker: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing worker by ID
        /// </summary>
        /// <param name="id">Worker ID</param>
        /// <param name="workerUpdateDto">Updated worker data</param>
        /// <returns>Updated worker</returns>
        /// <response code="200">Worker successfully updated</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Worker not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] WorkerUpdateDto workerUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingWorker = _databaseService.GetWorkerById(id);
                if (existingWorker == null)
                    return NotFound(new { message = "Worker not found" });

                _mapper.Map(workerUpdateDto, existingWorker);
                _databaseService.UpdateWorker(existingWorker);
                var updatedWorkerDto = _mapper.Map<WorkerResponseDto>(existingWorker);
                return Ok(updatedWorkerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating worker: " + ex.Message });
            }
        }

        /// <summary>
        /// Remove a worker by ID
        /// </summary>
        /// <param name="id">Worker ID</param>
        /// <response code="204">Worker successfully removed</response>
        /// <response code="404">Worker not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id) {
            try
            {
                var worker = _databaseService.GetWorkerById(id);
                if (worker == null)
                    return NotFound(new { message = "Worker not found" });

                _databaseService.DeleteWorker(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting worker: " + ex.Message });
            }
        }
    }
}