using AgroindustryManagementAPI.Services.Calculations;
using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgroManagementAPI.DTOs;
using AgroManagementAPI.Models;
using Asp.Versioning;
//using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace AgroManagementAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class FieldController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IAGCalculationService _aGCalculationService;
        public FieldController(IAGDatabaseService databaseService,IAGCalculationService aGCalculationService)
        {
            _databaseService = databaseService;
            _aGCalculationService = aGCalculationService;
        }
        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            try
            {
                var fields = _databaseService.GetAllFields();
                return Ok(fields);
            }
            catch (Exception)
            {
                //TempData["ErrorMessage"] = "Помилка при завантаженні полів: " + ex.Message;
                return NoContent();
            }

        }
        [HttpGet]
        [Route("details/{fieldId}")]
        public IActionResult Details(int fieldId)
        {
            try
            {
                var field = _databaseService.GetFieldById(fieldId);                
                return Ok(field);
            }
            catch (KeyNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }
        [HttpPost]
        [Route("create")]
        public IActionResult Create(FieldDTO createFieldDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var field = _databaseService.AddField(createFieldDTO);
                return CreatedAtAction(nameof(Details), new { fieldId = field.Id }, field);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("edit/{id}")]
        public IActionResult Edit(Field field)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _databaseService.UpdateField(field);
                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
               return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }

        }
        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _databaseService.DeleteField(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                
                return Conflict(ex.Message);

            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}