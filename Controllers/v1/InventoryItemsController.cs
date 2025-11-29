using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs.V1.InventoryItem;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
    /// <summary>
    /// Controller for managing inventory items in warehouses
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class InventoryItemsController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IMapper _mapper;

        public InventoryItemsController(IAGDatabaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all inventory items
        /// </summary>
        /// <returns>List of all inventory items</returns>
        /// <response code="200">Inventory items retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<InventoryItemResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var items = _databaseService.GetAllInventoryItems();
                var itemsDto = _mapper.Map<List<InventoryItemResponseDto>>(items);
                return Ok(itemsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inventory items: " + ex.Message });
            }
        }

        /// <summary>
        /// Get an inventory item by ID
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <returns>Inventory item data</returns>
        /// <response code="200">Inventory item retrieved successfully</response>
        /// <response code="404">Inventory item not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InventoryItemResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(int id)
        {
            try
            {
                var item = _databaseService.GetInventoryItemById(id);
                if (item == null)
                    return NotFound(new { message = "Inventory item not found" });

                var itemDto = _mapper.Map<InventoryItemResponseDto>(item);
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inventory item: " + ex.Message });
            }
        }

        /// <summary>
        /// Create a new inventory item
        /// </summary>
        /// <param name="itemCreateDto">Inventory item creation data</param>
        /// <returns>Created inventory item</returns>
        /// <response code="201">Inventory item created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(InventoryItemResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] InventoryItemCreateDto itemCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var item = _mapper.Map<InventoryItem>(itemCreateDto);
                _databaseService.AddInventoryItem(item);
                var createdItemDto = _mapper.Map<InventoryItemResponseDto>(item);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, createdItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating inventory item: " + ex.Message });
            }
        }

        /// <summary>
        /// Update an existing inventory item
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <param name="itemUpdateDto">Updated inventory item data</param>
        /// <returns>Updated inventory item</returns>
        /// <response code="200">Inventory item updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Inventory item not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(InventoryItemResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(int id, [FromBody] InventoryItemUpdateDto itemUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingItem = _databaseService.GetInventoryItemById(id);
                if (existingItem == null)
                    return NotFound(new { message = "Inventory item not found" });

                _mapper.Map(itemUpdateDto, existingItem);
                _databaseService.UpdateInventoryItem(existingItem);
                var updatedItemDto = _mapper.Map<InventoryItemResponseDto>(existingItem);
                return Ok(updatedItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating inventory item: " + ex.Message });
            }
        }

        /// <summary>
        /// Delete an inventory item
        /// </summary>
        /// <param name="id">Inventory item ID</param>
        /// <response code="204">Inventory item deleted successfully</response>
        /// <response code="404">Inventory item not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes. Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            try
            {
                var item = _databaseService.GetInventoryItemById(id);
                if (item == null)
                    return NotFound(new { message = "Inventory item not found" });

                _databaseService.DeleteInventoryItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting inventory item: " + ex.Message });
            }
        }
    }
}