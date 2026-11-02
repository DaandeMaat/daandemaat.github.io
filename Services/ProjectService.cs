using daandemaat.github.io.Data;
using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context) => _context = context;

    public async Task<List<Project>> GetAllAsync() =>
        await _context.Projecten.AsNoTracking().OrderByDescending(p => p.Datum).ToListAsync();

    public async Task<Project?> GetByIdAsync(int id) =>
        await _context.Projecten.FindAsync(id);

    public async Task<Project> CreateAsync(Project dto)
    {
        Validate(dto);

        var project = new Project
        {
            Titel = dto.Titel.Trim(),
            Beschrijving = dto.Beschrijving,
            Link = dto.Link,
            Categorie = dto.Categorie,
            Datum = dto.Datum
        };

        _context.Projecten.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> UpdateAsync(int id, Project dto)
    {
        Validate(dto);

        var project = await _context.Projecten.FindAsync(id);
        if (project is null) return false;

        project.Titel = dto.Titel.Trim();
        project.Beschrijving = dto.Beschrijving;
        project.Link = dto.Link;
        project.Categorie = dto.Categorie;
        project.Datum = dto.Datum;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projecten.FindAsync(id);
        if (project is null) return false;

        _context.Projecten.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    private static void Validate(Project dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titel))
            throw new ArgumentException("Titel is verplicht.");

        if (!Uri.TryCreate(dto.Link, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Link moet een geldige http(s)-url zijn.");
    }
}