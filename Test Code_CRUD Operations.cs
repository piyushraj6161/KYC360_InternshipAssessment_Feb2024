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
        // Endpoint to create a new entity
        [HttpPost]
        public IActionResult CreateEntity(Entity entity)
        {
            // Add the entity to the list
            entities.Add(entity);
            // Return a success response
            return Ok("Entity created successfully");
        }

        // Read Endpoints

        // Get All Entities
        // GET: /api/entities
        // Endpoint to fetch a list of all entities
        [HttpGet]
        public IActionResult GetAllEntities()
        {
            // Return all entities
            return Ok(entities);
        }

        // Get Single Entity by ID
        // GET: /api/entities/{id}
        // Endpoint to fetch a single entity by its ID
        [HttpGet("{id}")]
        public IActionResult GetEntityById(string id)
        {
            // Find the entity with the given ID
            var entity = entities.FirstOrDefault(e => e.Id == id);
            // If entity is found, return it
            if (entity != null)
            {
                return Ok(entity);
            }
            // If entity is not found, return 404 Not Found
            return NotFound();
        }

        // Update Entity
        // PUT: /api/entities/{id}
        // Endpoint to update an existing entity
        [HttpPut("{id}")]
        public IActionResult UpdateEntity(string id, Entity updatedEntity)
        {
            // Find the entity with the given ID
            var entity = entities.FirstOrDefault(e => e.Id == id);
            // If entity is found, update its properties
            if (entity != null)
            {
                entity.Addresses = updatedEntity.Addresses;
                entity.Dates = updatedEntity.Dates;
                entity.Deceased = updatedEntity.Deceased;
                entity.Gender = updatedEntity.Gender;
                entity.Names = updatedEntity.Names;
                // Return a success response
                return Ok("Entity updated successfully");
            }
            // If entity is not found, return 404 Not Found
            return NotFound();
        }

        // Delete Entity
        // DELETE: /api/entities/{id}
        // Endpoint to delete an existing entity by its ID
        [HttpDelete("{id}")]
        public IActionResult DeleteEntity(string id)
        {
            // Find the entity with the given ID
            var entity = entities.FirstOrDefault(e => e.Id == id);
            // If entity is found, remove it from the list
            if (entity != null)
            {
                entities.Remove(entity);
                // Return a success response
                return Ok("Entity deleted successfully");
            }
            // If entity is not found, return 404 Not Found
            return NotFound();
        }

        // Search and Filtering Endpoints

        // Search Entities
        // GET: /api/entities?search={query}
        // Endpoint to search for entities based on the provided search query
        [HttpGet("search")]
        public IActionResult SearchEntities(string search)
        {
            // Perform search across entities' properties
            var result = entities.Where(e =>
                e.Names.Any(n =>
                    n.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    n.MiddleName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    n.Surname.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                e.Addresses.Any(a =>
                    a.AddressLine.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Country.Contains(search, StringComparison.OrdinalIgnoreCase)));
            
            return Ok(result);
        }

        // Filter Entities
        // GET: /api/entities/filter?gender={gender}&startDate={startDate}&endDate={endDate}&countries={countries}
        // Endpoint to filter entities based on optional query parameters
        [HttpGet("filter")]
        public IActionResult FilterEntities(string gender, DateTime? startDate, DateTime? endDate, string[] countries)
        {
            // Filter entities based on the provided query parameters
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
