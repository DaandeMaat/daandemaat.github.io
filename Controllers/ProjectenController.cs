using daandemaat.github.io.Data;
using daandemaat.github.io.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectenController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectenController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/project
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjecten()
    {
        return await _context.Projecten.ToListAsync();
    }

    // GET: api/project/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(int id)
    {
        var project= await _context.Projecten.FindAsync(id);
        if (project == null) return NotFound();
        return project;
    }

    // POST: api/project
    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project project)
    {
        _context.Projecten.Add(project);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    // PUT: api/project/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(int id, Project project)
    {
        if (id != project.Id) return BadRequest();

        _context.Entry(project).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Projecten.AnyAsync(p => p.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/project/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projecten.FindAsync(id);
        if (project == null) return NotFound();

        _context.Projecten.Remove(project);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}