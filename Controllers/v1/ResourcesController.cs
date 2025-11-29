using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.Resource;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing agricultural resources (seeds, fertilizers, etc.)
    /// </summary>
    [ApiVersion("1.0")]
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
        /// <returns>List of all resources</returns>
        /// <response code="200">Resources retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResourceResponseDto>), StatusCodes. Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var resources = _databaseService.GetAllResources();
                var resourcesDto = _mapper.Map<List<ResourceResponseDto>>(resources);
                return Ok(resourcesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving resources: " + ex.Message });
            }
        }

        /// <summary>
        /// Get a resource by ID
        /// </summary>
        /// <param name="id">Resource ID</param>
        /// <returns>Resource data</returns>
        /// <response code="200">Resource retrieved successfully</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponseDto), StatusCodes. Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <param name="resourceCreateDto">Resource creation data</param>
        /// <returns>Created resource</returns>
        /// <response code="201">Resource created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResourceResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <param name="id">Resource ID</param>
        /// <param name="resourceUpdateDto">Updated resource data</param>
        /// <returns>Updated resource</returns>
        /// <response code="200">Resource updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResourceResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes. Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] ResourceUpdateDto resourceUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingResource = _databaseService.GetResourceById(id);
                if (existingResource == null)
                    return NotFound(new { message = "Resource not found" });

                _mapper.Map(resourceUpdateDto, existingResource);
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
        /// <param name="id">Resource ID</param>
        /// <response code="204">Resource deleted successfully</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            try
            {
                var resource = _databaseService.GetResourceById(id);
                if (resource == null)
                    return NotFound(new { message = "Resource not found" });

                _databaseService.DeleteResource(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting resource: " + ex.Message });
            }
        }
    }
}