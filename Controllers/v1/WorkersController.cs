using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services. Database;
using AgroManagementAPI.DTOs. V1. Worker;
using Asp.Versioning;
using Microsoft.AspNetCore. Mvc;

namespace AgroManagementAPI.Controllers.v1
{
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
        [HttpGet]
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
        /// Get worker by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var worker = _databaseService. GetWorkerById(id);
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
        [HttpPost]
        public IActionResult Create([FromBody] WorkerCreateDto workerCreateDto)
        {
            if (! ModelState.IsValid)
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
        /// Update an existing worker
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WorkerUpdateDto workerUpdateDto)
        {
            if (! ModelState.IsValid)
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
                return StatusCode(500, new { message = "Error updating worker: " + ex. Message });
            }
        }

        /// <summary>
        /// Delete a worker
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var worker = _databaseService.GetWorkerById(id);
                if (worker == null)
                    return NotFound(new { message = "Worker not found" });

                _databaseService. DeleteWorker(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting worker: " + ex.Message });
            }
        }
    }
}