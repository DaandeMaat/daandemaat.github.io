using daandemaat.github.io.Data;
using daandemaat.github.io.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Controllers;

[Route("api/blog")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly AppDbContext _context;

    public BlogController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/blog
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
    {
        return await _context.Blogs.ToListAsync();
    }

    // GET: api/blog/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Blog>> GetBlogById(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();
        return blog;
    }

    // POST: api/blog
    [HttpPost]
    public async Task<ActionResult<Blog>> PostBlog(Blog blog)
    {
        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBlogById), new { id = blog.Id }, blog);
    }

    // PUT: api/blog/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBlog(int id, Blog blog)
    {
        if (id != blog.Id) return BadRequest();

        _context.Entry(blog).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Blogs.AnyAsync(b => b.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    // DELETE: api/blog/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null) return NotFound();

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}