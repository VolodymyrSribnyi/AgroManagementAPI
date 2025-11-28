using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1. Resource;
using AgroManagementAPI.DTOs.V2.Resource;
using AgroManagementAPI.DTOs.V2. Common;
using Asp. Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AgroManagementAPI.Controllers. v2
{
    [ApiVersion("2.0")]
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
        /// Get all resources (V1 compatibility)
        /// </summary>
        [HttpGet]
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
                return StatusCode(500, new { message = "Error retrieving resources: " + ex. Message });
            }
        }

        /// <summary>
        /// Get resources with filtering and pagination (V2 NEW)
        /// </summary>
        [HttpGet("filter")]
        public IActionResult GetWithFilter([FromQuery] ResourceFilterDto filterDto)
        {
            try
            {
                var resources = _databaseService.GetAllResources();

                // Apply filters
                if (filterDto.CultureType.HasValue)
                    resources = resources.Where(r => r.CultureType == filterDto. CultureType.Value).ToList();

                if (filterDto. MinYield.HasValue)
                    resources = resources.Where(r => r. Yield >= filterDto.MinYield.Value).ToList();

                if (filterDto.MaxYield.HasValue)
                    resources = resources.Where(r => r.Yield <= filterDto.MaxYield.Value).ToList();

                // Pagination
                int pageNumber = filterDto.PageNumber ?? 1;
                int pageSize = filterDto.PageSize ?? 10;

                int totalCount = resources.Count();
                var paginatedResources = resources.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var resourcesDto = _mapper.Map<List<ResourceResponseDto>>(paginatedResources);

                var result = new PaginatedResultDto<ResourceResponseDto>
                {
                    Items = resourcesDto,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving resources: " + ex.Message });
            }
        }

        /// <summary>
        /// Get resource by ID (V1 compatibility)
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
        /// Create a new resource (V1 compatibility)
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
        /// Update an existing resource (V1 compatibility)
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
        /// Delete a resource (V1 compatibility)
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