[HttpGet]
public IActionResult GetEntities(int pageNumber = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
{
    // Validate input values for pagination
    if (pageNumber < 1)
    {
        return BadRequest("Invalid page number. Page number must be a positive integer.");
    }
    if (pageSize < 1)
    {
        return BadRequest("Invalid page size. Page size must be a positive integer.");
    }

    // Sort direction validation
    if (sortOrder.ToLower() != "asc" && sortOrder.ToLower() != "desc")
    {
        return BadRequest("Invalid sort order. Sort order must be either 'asc' or 'desc'.");
    }

    // Sort validation
    var validSortProperties = new List<string> { "Id", "Deceased", "Gender" }; // Add more if needed
    if (!validSortProperties.Contains(sortBy))
    {
        return BadRequest("Invalid sort by property. Allowed values: Id, Deceased, Gender.");
    }

    // Apply pagination
    var startIndex = (pageNumber - 1) * pageSize;
    var entitiesPage = entities.Skip(startIndex).Take(pageSize);

    // Apply sorting
    var sortedEntities = sortOrder.ToLower() == "desc" ?
        entitiesPage.OrderByDescending(e => GetPropertyValue(e, sortBy)) :
        entitiesPage.OrderBy(e => GetPropertyValue(e, sortBy));

    return Ok(sortedEntities);
}

// Helper method to get property value by property name using reflection
private object GetPropertyValue(Entity entity, string propertyName)
{
    var property = typeof(Entity).GetProperty(propertyName);
    return property?.GetValue(entity, null);
}
