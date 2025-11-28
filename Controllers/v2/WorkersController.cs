using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services. Database;
using AgroManagementAPI.DTOs. V1.Worker;
using AgroManagementAPI.DTOs.V2.Worker;
using AgroManagementAPI. DTOs.V2.Common;
using Asp.Versioning;
using Microsoft.AspNetCore. Mvc;


namespace AgroManagementAPI.Controllers. v2
{
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
        /// Get all workers (V1 compatibility)
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
        /// Get workers with filtering and pagination (V2 NEW)
        /// </summary>
        [HttpGet("filter")]
        public IActionResult GetWithFilter([FromQuery] WorkerFilterDto filterDto)
        {
            try
            {
                var workers = _databaseService.GetAllWorkers();

                // Apply filters
                if (filterDto.IsActive. HasValue)
                    workers = workers.Where(w => w. IsActive == filterDto.IsActive.Value).ToList();

                if (filterDto.MinAge.HasValue)
                    workers = workers.Where(w => w.Age >= filterDto.MinAge.Value).ToList();

                if (filterDto.MaxAge.HasValue)
                    workers = workers. Where(w => w.Age <= filterDto.MaxAge.Value). ToList();

                if (filterDto.MinHourlyRate.HasValue)
                    workers = workers.Where(w => w.HourlyRate >= filterDto.MinHourlyRate.Value).ToList();

                if (filterDto.MaxHourlyRate.HasValue)
                    workers = workers.Where(w => w.HourlyRate <= filterDto.MaxHourlyRate.Value).ToList();

                // Pagination
                int pageNumber = filterDto.PageNumber ?? 1;
                int pageSize = filterDto.PageSize ??  10;

                int totalCount = workers.Count();
                var paginatedWorkers = workers.Skip((pageNumber - 1) * pageSize). Take(pageSize).ToList();

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
        /// Get worker by ID (V1 compatibility)
        /// </summary>
        [HttpGet("{id}")]
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
        /// Create a new worker (V1 compatibility)
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
        /// Update an existing worker (V1 compatibility)
        /// </summary>
        [HttpPut("{id}")]
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
        /// Delete a worker (V1 compatibility)
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