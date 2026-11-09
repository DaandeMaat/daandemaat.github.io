using daandemaat.github.io.Models;
using daandemaat.github.io.Services;
using Microsoft.AspNetCore.Mvc;

namespace daandemaat.github.io.Controllers;

[Route("api/test")]
[ApiController]

public class TestController : ControllerBase
{
    private readonly ITestService _service;

    public TestController(ITestService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<Test>>> GetTests() =>
        await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Test>> GetTestById(int id)
    {
        var test = await _service.GetByIdAsync(id);
        if (test is null) return NotFound();
        return test;
    }

    [HttpPost]
    public async Task<ActionResult<Test>> PostTest(Test dto)
    {
        try
        {
            var test = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTestById), new { }, test);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTest(int id, Test dto)
    {
        try
        {
            return await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTest(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}