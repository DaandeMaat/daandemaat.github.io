using daandemaat.github.io.Models;
using daandemaat.github.io.Services;
using Microsoft.AspNetCore.Mvc;

namespace daandemaat.github.io.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectenController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetProjecten() =>
        await service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(int id)
    {
        var project = await service.GetByIdAsync(id);
        if (project is null) return NotFound();
        return project;
    }

    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project dto)
    {
        try
        {
            var project = await service.CreateAsync(dto);
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
            return await service.UpdateAsync(id, dto) ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id) =>
        await service.DeleteAsync(id) ? NoContent() : NotFound();
}