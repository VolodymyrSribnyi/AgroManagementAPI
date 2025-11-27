using AgroindustryManagementAPI.Models;
using AgroindustryManagementAPI.Services.Database;
using Microsoft.AspNetCore.Mvc;

namespace AgroManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IAGDatabaseService _databaseService;
        public ResourceController(IAGDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        [HttpGet]
        [Route("/api/resource/index")]
        public IActionResult Index()
        {
            try
            {
                var resources = _databaseService.GetAllResources();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Помилка при завантаженні ресурсів: " + ex.Message);

            }
        }
        [HttpGet]
        [Route("/api/resource/details/{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                var resource = _databaseService.GetResourceById(id);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                
                return StatusCode(500, ex.Message); 
            }
        }
        [HttpPost]
        [Route("/api/resource/create")]
        public IActionResult Create(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return Ok(resource);
            }
            try
            {
                _databaseService.AddResource(resource);
                return Ok("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
        [HttpPost]
        [Route("/api/resource/edit/{id}")]
        public IActionResult Edit(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(resource);
            }
            try
            {
                _databaseService.UpdateResource(resource);
                return Ok(resource);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        //public IActionResult Delete(int id)
        //{
        //    try
        //    {
        //        var resource = _databaseService.GetResourceById(id);
        //        return Ok(resource);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
                
        //        return NotFound(ex.Message);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }

        //}
        [HttpDelete]
        [Route("/api/resource/delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _databaseService.DeleteResource(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
