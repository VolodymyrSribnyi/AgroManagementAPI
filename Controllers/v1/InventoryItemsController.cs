using AutoMapper;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI. Services.Database;
using AgroManagementAPI.DTOs. V1.InventoryItem;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v1
{
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
        [HttpGet]
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
        /// Get inventory item by ID
        /// </summary>
        [HttpGet("{id}")]
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
        [HttpPost]
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
        [HttpPut("{id}")]
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
                _databaseService. UpdateInventoryItem(existingItem);
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
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var item = _databaseService. GetInventoryItemById(id);
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