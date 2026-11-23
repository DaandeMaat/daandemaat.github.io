using daandemaat.github.io.Models;
using daandemaat.github.io.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace daandemaat.github.io.Controllers;

[Route("api/blog")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly IBlogService _service;

    public BlogController(IBlogService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<Blog>>> GetBlogs() =>
        await _service.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Blog>> GetBlogById(int id)
    {
        var blog = await _service.GetByIdAsync(id);
        if (blog is null) return NotFound();
        return Ok(blog);
    }

    [Authorize(Policy = "CanEditBlogs")]
    [HttpPost]
    public async Task<ActionResult<Blog>> PostBlog(Blog dto)
    {
        try
        {
            var blog = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, blog);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "CanEditBlogs")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBlog(int id, Blog dto)
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

    [Authorize(Policy = "CanEditBlogs")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}