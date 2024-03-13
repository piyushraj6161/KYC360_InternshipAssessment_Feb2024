using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YourNamespace.Controllers
{
    [Route("api/entities")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        // Mocked database representing entities
        private static List<Entity> entities = new List<Entity>();

        // CRUD Endpoints

        // Create Entity
        // POST: /api/entities
        [HttpPost]
        public IActionResult CreateEntity(Entity entity)
        {
            entities.Add(entity);
            return CreatedAtAction(nameof(GetEntityById), new { id = entity.Id }, entity);
        }

        // Read Endpoints

        // Get All Entities
        // GET: /api/entities
        [HttpGet]
        public IActionResult GetAllEntities()
        {
            return Ok(entities);
        }

        // Get Single Entity by ID
        // GET: /api/entities/{id}
        [HttpGet("{id}")]
        public IActionResult GetEntityById(string id)
        {
            var entity = entities.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                return Ok(entity);
            }
            return NotFound();
        }

        // Update Entity
        // PUT: /api/entities/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateEntity(string id, Entity updatedEntity)
        {
            var entity = entities.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                // Update entity properties
                entity.Addresses = updatedEntity.Addresses;
                entity.Dates = updatedEntity.Dates;
                entity.Deceased = updatedEntity.Deceased;
                entity.Gender = updatedEntity.Gender;
                entity.Names = updatedEntity.Names;
                return Ok("Entity updated successfully");
            }
            return NotFound();
        }

        // Delete Entity
        // DELETE: /api/entities/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEntity(string id)
        {
            var entity = entities.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                entities.Remove(entity);
                return Ok("Entity deleted successfully");
            }
            return NotFound();
        }

        // Search and Filtering Endpoints

        // Search Entities
        // GET: /api/entities/search?text={searchText}
        [HttpGet("search")]
        public IActionResult SearchEntities(string text)
        {
            var result = entities.Where(e =>
                e.Names.Any(n =>
                    n.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    n.MiddleName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    n.Surname.Contains(text, StringComparison.OrdinalIgnoreCase)) ||
                e.Addresses.Any(a =>
                    a.AddressLine.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    a.Country.Contains(text, StringComparison.OrdinalIgnoreCase)));

            return Ok(result);
        }

        // Filter Entities
        // GET: /api/entities/filter?gender={gender}&startDate={startDate}&endDate={endDate}&countries={countries}
        [HttpGet("filter")]
        public IActionResult FilterEntities(string gender, DateTime? startDate, DateTime? endDate, string[] countries)
        {
            var result = entities.Where(e =>
                (gender == null || e.Gender == gender) &&
                (startDate == null || e.Dates.Any(d => d.Date >= startDate)) &&
                (endDate == null || e.Dates.Any(d => d.Date <= endDate)) &&
                (countries == null || !countries.Except(e.Addresses.Select(a => a.Country)).Any()));

            return Ok(result);
        }
    }

    // Entity model classes
    public interface IEntity
    {
        List<Address>? Addresses { get; set; }
        List<Date> Dates { get; set; }
        bool Deceased { get; set; }
        string Id { get; set; }
        List<Name> Names { get; set; }
    }

    public class Entity : IEntity
    {
        public List<Address>? Addresses { get; set; }
        public List<Date> Dates { get; set; }
        public bool Deceased { get; set; }
        public string Id { get; set; }
        public List<Name> Names { get; set; }
    }

    public class Address
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class Date
    {
        public string? DateType { get; set; }
        public DateTime? DateValue { get; set; }
    }

    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
    }
}
