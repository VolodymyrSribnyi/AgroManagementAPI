using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.WorkerTask;
using AgroManagementAPI.DTOs.V2.WorkerTask;
using AgroManagementAPI.DTOs.V2. Common;
using Asp. Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AgroManagementAPI.Controllers. v2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class WorkerTasksController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public WorkerTasksController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all worker tasks (V1 compatibility)
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var tasks = _databaseService.GetAllWorkerTasks();
                var tasksDto = _mapper. Map<List<WorkerTaskResponseDto>>(tasks);
                return Ok(tasksDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tasks: " + ex.Message });
            }
        }

        /// <summary>
        /// Get worker tasks with filtering and pagination (V2 NEW)
        /// </summary>
        [HttpGet("filter")]
        public IActionResult GetWithFilter([FromQuery] WorkerTaskFilterDto filterDto)
        {
            try
            {
                var tasks = _databaseService.GetAllWorkerTasks();

                // Apply filters
                if (filterDto.TaskType.HasValue)
                    tasks = tasks.Where(t => t.TaskType == filterDto. TaskType.Value).ToList();

                if (filterDto.MinProgress.HasValue)
                    tasks = tasks.Where(t => t.Progress >= filterDto.MinProgress.Value).ToList();

                if (filterDto.MaxProgress. HasValue)
                    tasks = tasks.Where(t => t. Progress <= filterDto.MaxProgress. Value).ToList();

                if (filterDto.WorkerId.HasValue)
                    tasks = tasks.Where(t => t.WorkerId == filterDto.WorkerId.Value).ToList();

                if (filterDto. FieldId.HasValue)
                    tasks = tasks.Where(t => t.FieldId == filterDto. FieldId.Value).ToList();

                // Pagination
                int pageNumber = filterDto.PageNumber ?? 1;
                int pageSize = filterDto.PageSize ?? 10;

                int totalCount = tasks.Count();
                var paginatedTasks = tasks.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var tasksDto = _mapper.Map<List<WorkerTaskResponseDto>>(paginatedTasks);

                var result = new PaginatedResultDto<WorkerTaskResponseDto>
                {
                    Items = tasksDto,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tasks: " + ex.Message });
            }
        }

        /// <summary>
        /// Get worker task by ID (V1 compatibility)
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var task = _databaseService. GetWorkerTaskById(id);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                var taskDto = _mapper.Map<WorkerTaskResponseDto>(task);
                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving task: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new worker task (V1 compatibility)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] WorkerTaskCreateDto taskCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var task = _mapper.Map<WorkerTask>(taskCreateDto);
                _databaseService. AddWorkerTask(task);
                var createdTaskDto = _mapper.Map<WorkerTaskResponseDto>(task);
                return CreatedAtAction(nameof(GetById), new { id = task. Id }, createdTaskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating task: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing worker task (V1 compatibility)
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WorkerTaskUpdateDto taskUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingTask = _databaseService.GetWorkerTaskById(id);
                if (existingTask == null)
                    return NotFound(new { message = "Task not found" });

                _mapper.Map(taskUpdateDto, existingTask);
                _databaseService.UpdateWorkerTask(existingTask);
                var updatedTaskDto = _mapper.Map<WorkerTaskResponseDto>(existingTask);
                return Ok(updatedTaskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating task: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a worker task (V1 compatibility)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var task = _databaseService.GetWorkerTaskById(id);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                _databaseService.DeleteWorkerTask(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting task: " + ex.Message });
            }
        }
    }
}