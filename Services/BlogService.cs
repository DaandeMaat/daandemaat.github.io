using daandemaat.github.io.Data;
using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Services;

public class BlogService : IBlogService
{
    private readonly AppDbContext _context;

    public BlogService(AppDbContext context) => _context = context;

    public async Task<List<Blog>> GetAllAsync() =>
        await _context.Blogs.AsNoTracking().OrderByDescending(b => b.Datum).ToListAsync();

    public async Task<Blog?> GetByIdAsync(int id) =>
        await _context.Blogs.FindAsync(id);

    public async Task<Blog> CreateAsync(Blog dto)
    {
        Validate(dto);

        var blog = new Blog
        {
            Titel = dto.Titel.Trim(),
            Inhoud = dto.Inhoud,
            Datum = dto.Datum
        };

        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();
        return blog;
    }

    public async Task<bool> UpdateAsync(int id, Blog dto)
    {
        Validate(dto);

        var blog = await _context.Blogs.FindAsync(id);
        if (blog is null) return false;

        blog.Titel = dto.Titel.Trim();
        blog.Inhoud = dto.Inhoud;
        blog.Datum = dto.Datum;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog is null) return false;

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return true;
    }

    private static void Validate(Blog dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titel))
            throw new ArgumentException("Titel is verplicht.");

        if (string.IsNullOrWhiteSpace(dto.Inhoud))
            throw new ArgumentException("Inhoud is verplicht.");

        if (dto.Datum > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Datum mag niet in de toekomst liggen.");
    }
}