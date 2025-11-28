using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1. Resource;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    [ApiVersion("1. 0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public ResourcesController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all resources
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var resources = _databaseService. GetAllResources();
                var resourcesDto = _mapper.Map<List<ResourceResponseDto>>(resources);
                return Ok(resourcesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving resources: " + ex.Message });
            }
        }

        /// <summary>
        /// Get resource by ID
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var resource = _databaseService.GetResourceById(id);
                if (resource == null)
                    return NotFound(new { message = "Resource not found" });

                var resourceDto = _mapper.Map<ResourceResponseDto>(resource);
                return Ok(resourceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving resource: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new resource
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] ResourceCreateDto resourceCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resource = _mapper.Map<Resource>(resourceCreateDto);
                _databaseService.AddResource(resource);
                var createdResourceDto = _mapper.Map<ResourceResponseDto>(resource);
                return CreatedAtAction(nameof(GetById), new { id = resource.Id }, createdResourceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating resource: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing resource
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ResourceUpdateDto resourceUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingResource = _databaseService.GetResourceById(id);
                if (existingResource == null)
                    return NotFound(new { message = "Resource not found" });

                _mapper. Map(resourceUpdateDto, existingResource);
                _databaseService.UpdateResource(existingResource);
                var updatedResourceDto = _mapper.Map<ResourceResponseDto>(existingResource);
                return Ok(updatedResourceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating resource: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a resource
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var resource = _databaseService.GetResourceById(id);
                if (resource == null)
                    return NotFound(new { message = "Resource not found" });

                _databaseService. DeleteResource(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting resource: " + ex.Message });
            }
        }
    }
}