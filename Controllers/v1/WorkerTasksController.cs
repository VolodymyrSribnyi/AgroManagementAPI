using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.WorkerTask;
using Asp. Versioning;
using Microsoft. AspNetCore.Mvc;

namespace AgroManagementAPI. Controllers.v1
{
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
        [HttpGet]
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
                return StatusCode(500, new { message = "Error retrieving tasks: " + ex.Message });
            }
        }

        /// <summary>
        /// Get worker task by ID
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
        /// Create a new worker task
        /// </summary>
        [HttpPost]
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
                return StatusCode(500, new { message = "Error creating task: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing worker task
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WorkerTaskUpdateDto taskUpdateDto)
        {
            if (! ModelState.IsValid)
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
        /// Delete a worker task
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var task = _databaseService. GetWorkerTaskById(id);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                _databaseService. DeleteWorkerTask(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting task: " + ex. Message });
            }
        }
    }
}