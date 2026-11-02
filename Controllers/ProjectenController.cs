using daandemaat.github.io.Models;
using daandemaat.github.io.Services;
using Microsoft.AspNetCore.Mvc;

namespace daandemaat.github.io.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectenController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectenController(IProjectService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetProjecten() =>
        await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(int id)
    {
        var project = await _service.GetByIdAsync(id);
        if (project is null) return NotFound();
        return project;
    }

    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project dto)
    {
        try
        {
            var project = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(int id, Project dto)
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
    public async Task<IActionResult> DeleteProject(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}