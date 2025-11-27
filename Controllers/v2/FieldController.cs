using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Calculations;
using AgroindustryManagementAPI.Services.Database;
using AgroManagementAPI.DTOs;
using AgroManagementAPI.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class FieldController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        private readonly IAGCalculationService _aGCalculationService;
        public FieldController(IAGDatabaseService databaseService, IAGCalculationService aGCalculationService)
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
                var resource = _databaseService.GetResourceByCultureType(field.Culture);
                var workersCount = _aGCalculationService.CalculateRequiredWorkers(resource, field.Area);
                var RequiredWorkers = _aGCalculationService.CalculateRequiredWorkers(resource, field.Area);
                var RequiredMachinery = _aGCalculationService.CalculateRequiredMachineryCount(resource);
                var SeedAmount = _aGCalculationService.CalculateSeedAmount(resource, field.Area);
                var FertilizerAmount = _aGCalculationService.CalculateFertilizerAmount(resource, field.Area);
                var EstimatedYield = _aGCalculationService.EstimateYield(resource, field.Area);

                double fuelConsumption = 0;
                foreach (var machine in field.Machines)
                {
                    fuelConsumption += _aGCalculationService.EstimateFuelConsumption(machine, field.Area);
                }
                var EstimatedFuelConsumption = fuelConsumption;
                double duration = 0;
                foreach (var machine in field.Machines)
                {
                    duration += _aGCalculationService.EstimateWorkDuration(field.Area, workersCount, machine, resource);
                }
                var EstimateWorkDuration = Math.Ceiling(duration);
                var fieldDetails = new FieldDetails
                {
                    Id = field.Id,
                    Area = field.Area,
                    Culture = field.Culture,
                    Status = field.Status,
                    Workers = field.Workers,
                    Machines = field.Machines,
                    Tasks = field.Tasks,
                    FuelNeeded = EstimatedFuelConsumption,
                    WorkDuralityNeeded = EstimateWorkDuration,
                    FertilizerAmount = FertilizerAmount,
                    RequiredMachines = RequiredMachinery,
                    RequiredWorkers = RequiredWorkers,
                    SeedAmount = SeedAmount,
                    Yield = EstimatedYield
                };
                return Ok(fieldDetails);
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
                return RedirectToAction("Index");
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
