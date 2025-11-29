using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.Worker;
using AgroManagementAPI.DTOs.V2.Worker;
using AgroManagementAPI.DTOs.V2.Common;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v2
{
    /// <summary>
    /// Controller for managing workers (V2 - with filtering and pagination)
    /// </summary>
    [ApiVersion("2.0")]
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
        /// Get all workers (compatible with V1)
        /// </summary>
        /// <returns>List of all workers</returns>
        /// <response code="200">List of workers successfully retrieved</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<WorkerResponseDto>), StatusCodes.Status200OK)]
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
        /// Get workers with filtering and pagination (V2 NEW)
        /// </summary>
        /// <param name="filterDto">Filtering and pagination parameters</param>
        /// <returns>Page of workers based on filters</returns>
        /// <response code="200">Workers successfully retrieved</response>
        /// <remarks>
        /// Example request:
        /// 
        ///     GET /api/v2/workers/filter?isActive=true&minAge=25&pageNumber=1&pageSize=10
        /// </remarks>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(PaginatedResultDto<WorkerResponseDto>), StatusCodes.Status200OK)]
        public IActionResult GetWithFilter([FromQuery] WorkerFilterDto filterDto)
        {
            try
            {
                var workers = _databaseService.GetAllWorkers();

                if (filterDto.IsActive.HasValue)
                    workers = workers.Where(w => w.IsActive == filterDto.IsActive.Value).ToList();

                if (filterDto.MinAge.HasValue)
                    workers = workers.Where(w => w.Age >= filterDto.MinAge.Value).ToList();

                if (filterDto.MaxAge.HasValue)
                    workers = workers.Where(w => w.Age <= filterDto.MaxAge.Value).ToList();

                if (filterDto.MinHourlyRate.HasValue)
                    workers = workers.Where(w => w.HourlyRate >= filterDto.MinHourlyRate.Value).ToList();

                if (filterDto.MaxHourlyRate.HasValue)
                    workers = workers.Where(w => w.HourlyRate <= filterDto.MaxHourlyRate.Value).ToList();

                int pageNumber = filterDto.PageNumber ?? 1;
                int pageSize = filterDto.PageSize ?? 10;

                int totalCount = workers.Count();
                var paginatedWorkers = workers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var workersDto = _mapper.Map<List<WorkerResponseDto>>(paginatedWorkers);

                var result = new PaginatedResultDto<WorkerResponseDto>
                {
                    Items = workersDto,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <param name="workerCreateDto">Data for creating a worker</param>
        /// <returns>Created worker</returns>
        [HttpPost]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// Update a worker
        /// </summary>
        /// <param name="id">Worker ID</param>
        /// <param name="workerUpdateDto">Updated data</param>
        /// <returns>Updated worker</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WorkerResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// Delete a worker
        /// </summary>
        /// <param name="id">Worker ID</param>
        /// <response code="204">Successfully deleted</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
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