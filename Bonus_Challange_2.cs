using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EntityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private static List<Entity> entities = new List<Entity>(); // Mocked database

        // Retry Mechanism:
        public IActionResult WriteToDatabase(Entity entity)
        {
            int maxRetryAttempts = 3;
            int currentAttempt = 1;

            while (currentAttempt <= maxRetryAttempts)
            {
                try
                {
                    // Attempt to write to the database
                    WriteToDatabaseOperation(entity);
                    return Ok("Write operation successful");
                }
                catch (Exception ex)
                {
                    // Log the retry attempt
                    LogRetryAttempt(currentAttempt, ex.Message);
                    currentAttempt++;
                    // Wait before retrying
                    Thread.Sleep(TimeSpan.FromSeconds(2 * currentAttempt)); // Exponential backoff
                }
            }

            // If all retry attempts fail
            LogRetryAttemptFailed();
            return StatusCode(500, "Failed to write to the database after multiple retry attempts");
        }

        // Backoff Strategy:
        private void LogRetryAttempt(int attemptNumber, string errorMessage)
        {
            Console.WriteLine($"Retry Attempt {attemptNumber}: {errorMessage}");
        }

        private void LogRetryAttemptFailed()
        {
            Console.WriteLine("All retry attempts failed.");
        }

        // Logging:
        public class DatabaseLogger
        {
            public void LogRetryAttempt(int attemptNumber, TimeSpan delay)
            {
                Console.WriteLine($"Retry Attempt {attemptNumber}: Retrying after {delay.TotalSeconds} seconds");
            }

            public void LogRetrySuccess()
            {
                Console.WriteLine("Write operation succeeded after retrying.");
            }

            public void LogRetryFailure()
            {
                Console.WriteLine("All retry attempts failed.");
            }
        }

        // Test Cases:
        public void TestWriteToDatabase()
        {
            var entity = new Entity(); // Initialize entity with necessary data
            var databaseLogger = new DatabaseLogger();

            // Simulate initial failure
            MockDatabase.SetShouldFail(true);

            var result = WriteToDatabase(entity, databaseLogger);

            // Simulate success after retry
            MockDatabase.SetShouldFail(false);

            // Assert that the result indicates success
            Debug.Assert(result.Equals("Write operation successful"));
        }

        // GET: api/entities
        [HttpGet]
        public IActionResult GetAllEntities(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            // Apply pagination
            var entitiesSubset = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Apply sorting
            var propertyInfo = typeof(Entity).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo != null)
            {
                entitiesSubset = sortOrder.ToLower() == "desc" ? entitiesSubset.OrderByDescending(e => propertyInfo.GetValue(e, null)) : entitiesSubset.OrderBy(e => propertyInfo.GetValue(e, null));
            }
            else
            {
                // If sortBy parameter is invalid, default to sorting by Id
                entitiesSubset = entitiesSubset.OrderBy(e => e.Id);
            }

            return Ok(entitiesSubset);
        }

        // Helper method for database write operation
        private void WriteToDatabaseOperation(Entity entity)
        {
            // Simulate database write operation
            if (MockDatabase.ShouldFail())
            {
                throw new Exception("Database write operation failed");
            }
            else
            {
                entities.Add(entity);
            }
        }
    }

    // Mock database for testing purposes
    public static class MockDatabase
    {
        private static bool shouldFail = false;

        public static bool ShouldFail()
        {
            return shouldFail;
        }

        public static void SetShouldFail(bool value)
        {
            shouldFail = value;
        }
    }

    // Entity schema
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
