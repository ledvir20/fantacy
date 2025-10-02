using Fantacy.Backend.Data;
using Fantacy.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fantacy.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly DataContext _db;
    public CountriesController(DataContext context)
    {
        _db = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var countries = await _db.Countries.ToListAsync();
        return Ok(countries);
    }

    [HttpGet("{id:int}", Name = "GetCountryById")]
    public async Task<IActionResult> GetCountryByIdAsync(int id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }
        return Ok(country);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] Country country)
    {
        if (country == null || string.IsNullOrWhiteSpace(country.Name))
        {
            return BadRequest("Country name is required.");
        }

        var existingCountry = await _db.Countries
            .FirstOrDefaultAsync(c => c.Name.ToLower() == country.Name.ToLower());

        if (existingCountry != null)
        {
            return Conflict($"A country with the same name '{country.Name}' already exists.");
        }

        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        Console.WriteLine(country.Id);

        return CreatedAtAction("GetCountryById", new { id = country.Id }, country);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync([FromBody] Country country)
    {
        if (country == null || country.Id <= 0 || string.IsNullOrWhiteSpace(country.Name))
        {
            return BadRequest("Valid country ID and name are required.");
        }

        var existingCountry = await _db.Countries.FindAsync(country.Id);
        if (existingCountry == null)
        {
            return NotFound();
        }

        var duplicateCountry = await _db.Countries
            .FirstOrDefaultAsync(c => c.Name.ToLower() == country.Name.ToLower() && c.Id != country.Id);

        if (duplicateCountry != null)
        {
            return Conflict($"A country with the same name '{country.Name}' already exists.");
        }

        existingCountry.Name = country.Name;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}
