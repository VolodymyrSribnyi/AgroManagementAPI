using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.WorkerTask;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing worker tasks and assignments
    /// </summary>
    [ApiVersion("1.0")]
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
        /// Get all worker tasks
        /// </summary>
        /// <returns>List of all worker tasks</returns>
        /// <response code="200">Worker tasks retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<WorkerTaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var tasks = _databaseService.GetAllWorkerTasks();
                var tasksDto = _mapper.Map<List<WorkerTaskResponseDto>>(tasks);
                return Ok(tasksDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker tasks: " + ex.Message });
            }
        }

        /// <summary>
        /// Get a worker task by ID
        /// </summary>
        /// <param name="id">Worker task ID</param>
        /// <returns>Worker task data</returns>
        /// <response code="200">Worker task retrieved successfully</response>
        /// <response code="404">Worker task not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WorkerTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes. Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var task = _databaseService. GetWorkerTaskById(id);
                if (task == null)
                    return NotFound(new { message = "Worker task not found" });

                var taskDto = _mapper.Map<WorkerTaskResponseDto>(task);
                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving worker task: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new worker task
        /// </summary>
        /// <param name="taskCreateDto">Worker task creation data</param>
        /// <returns>Created worker task</returns>
        /// <response code="201">Worker task created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(WorkerTaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] WorkerTaskCreateDto taskCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var task = _mapper.Map<WorkerTask>(taskCreateDto);
                _databaseService.AddWorkerTask(task);
                var createdTaskDto = _mapper.Map<WorkerTaskResponseDto>(task);
                return CreatedAtAction(nameof(GetById), new { id = task.Id }, createdTaskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating worker task: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing worker task
        /// </summary>
        /// <param name="id">Worker task ID</param>
        /// <param name="taskUpdateDto">Updated worker task data</param>
        /// <returns>Updated worker task</returns>
        /// <response code="200">Worker task updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Worker task not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WorkerTaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes. Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] WorkerTaskUpdateDto taskUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingTask = _databaseService.GetWorkerTaskById(id);
                if (existingTask == null)
                    return NotFound(new { message = "Worker task not found" });

                _mapper.Map(taskUpdateDto, existingTask);
                _databaseService.UpdateWorkerTask(existingTask);
                var updatedTaskDto = _mapper.Map<WorkerTaskResponseDto>(existingTask);
                return Ok(updatedTaskDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating worker task: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a worker task
        /// </summary>
        /// <param name="id">Worker task ID</param>
        /// <response code="204">Worker task deleted successfully</response>
        /// <response code="404">Worker task not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            try
            {
                var task = _databaseService.GetWorkerTaskById(id);
                if (task == null)
                    return NotFound(new { message = "Worker task not found" });

                _databaseService.DeleteWorkerTask(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting worker task: " + ex.Message });
            }
        }
    }
}