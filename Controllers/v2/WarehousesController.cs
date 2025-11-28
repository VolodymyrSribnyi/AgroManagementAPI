using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1. Warehouse;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI. Controllers.v2
{
    [ApiVersion("2.0")]
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
        /// Get all warehouses (V1 compatibility)
        /// </summary>
        [HttpGet]
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
        /// Get warehouse by ID (V1 compatibility)
        /// </summary>
        [HttpGet("{id}")]
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
        /// Create a new warehouse (V1 compatibility)
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] WarehouseCreateDto warehouseCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var warehouse = _mapper.Map<Warehouse>(warehouseCreateDto);
                _databaseService. AddWarehouse(warehouse);
                var createdWarehouseDto = _mapper.Map<WarehouseResponseDto>(warehouse);
                return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, createdWarehouseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating warehouse: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing warehouse (V1 compatibility)
        /// </summary>
        [HttpPut("{id}")]
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
                var updatedWarehouseDto = _mapper. Map<WarehouseResponseDto>(existingWarehouse);
                return Ok(updatedWarehouseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating warehouse: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete a warehouse (V1 compatibility)
        /// </summary>
        [HttpDelete("{id}")]
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