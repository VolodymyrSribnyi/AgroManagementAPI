using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.Warehouse;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing warehouses and storage facilities
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public WarehousesController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all warehouses
        /// </summary>
        /// <returns>List of all warehouses</returns>
        /// <response code="200">Warehouses retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<WarehouseResponseDto>), StatusCodes. Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var warehouses = _databaseService.GetAllWarehouses();
                var warehousesDto = _mapper.Map<List<WarehouseResponseDto>>(warehouses);
                return Ok(warehousesDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving warehouses: " + ex.Message });
            }
        }

        /// <summary>
        /// Get a warehouse by ID
        /// </summary>
        /// <param name="id">Warehouse ID</param>
        /// <returns>Warehouse data</returns>
        /// <response code="200">Warehouse retrieved successfully</response>
        /// <response code="404">Warehouse not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var warehouse = _databaseService.GetWarehouseById(id);
                if (warehouse == null)
                    return NotFound(new { message = "Warehouse not found" });

                var warehouseDto = _mapper.Map<WarehouseResponseDto>(warehouse);
                return Ok(warehouseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving warehouse: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new warehouse
        /// </summary>
        /// <param name="warehouseCreateDto">Warehouse creation data</param>
        /// <returns>Created warehouse</returns>
        /// <response code="201">Warehouse created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] WarehouseCreateDto warehouseCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var warehouse = _mapper.Map<Warehouse>(warehouseCreateDto);
                _databaseService.AddWarehouse(warehouse);
                var createdWarehouseDto = _mapper.Map<WarehouseResponseDto>(warehouse);
                return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, createdWarehouseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating warehouse: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing warehouse
        /// </summary>
        /// <param name="id">Warehouse ID</param>
        /// <param name="warehouseUpdateDto">Updated warehouse data</param>
        /// <returns>Updated warehouse</returns>
        /// <response code="200">Warehouse updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Warehouse not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WarehouseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes. Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] WarehouseUpdateDto warehouseUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingWarehouse = _databaseService.GetWarehouseById(id);
                if (existingWarehouse == null)
                    return NotFound(new { message = "Warehouse not found" });

                _mapper.Map(warehouseUpdateDto, existingWarehouse);
                _databaseService.UpdateWarehouse(existingWarehouse);
                var updatedWarehouseDto = _mapper.Map<WarehouseResponseDto>(existingWarehouse);
                return Ok(updatedWarehouseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating warehouse: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a warehouse
        /// </summary>
        /// <param name="id">Warehouse ID</param>
        /// <response code="204">Warehouse deleted successfully</response>
        /// <response code="404">Warehouse not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            try
            {
                var warehouse = _databaseService.GetWarehouseById(id);
                if (warehouse == null)
                    return NotFound(new { message = "Warehouse not found" });

                _databaseService.DeleteWarehouse(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting warehouse: " + ex.Message });
            }
        }
    }
}