using System.Text.Json;
using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Data;

public static class DbSeeder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private record BlogSeed(string Titel, string Inhoud, DateOnly Datum);
    private record ProjectSeed(string Titel, string Beschrijving, string Link, string Categorie, DateOnly Datum);
    private record TestSeed(string Onderdeel, string Beschrijving, string Ezelsbrug);

    public static async Task SeedAsync(AppDbContext context, string contentRootPath)
    {
        var seedPath = Path.Combine(contentRootPath, "Data", "Seed");
        // Plakt het pad aan elkaar: <projectmap>\Data\Seed

        if (!await context.Blogs.AnyAsync())  // Alleen doorgaan als de tabel leeg is (AnyAsync = "staat er minstens één rij in?").
        {
            var blogs = await LoadAsync<BlogSeed>(Path.Combine(seedPath, "blogs.json"));
            context.Blogs.AddRange(blogs.Select(b => new Blog
            {
                Titel = b.Titel,
                Inhoud = b.Inhoud,
                Datum = b.Datum
            }));
            // Select maakt van elk BlogSeed een echt Blog-model. AddRange zet ze allemaal klaar om op te slaan.
        }

        if (!await context.Projecten.AnyAsync())
        {
            var projecten = await LoadAsync<ProjectSeed>(Path.Combine(seedPath, "projecten.json"));
            context.Projecten.AddRange(projecten.Select(p => new Project
            {
                Titel = p.Titel,
                Beschrijving = p.Beschrijving,
                Link = p.Link,
                Categorie = p.Categorie,
                Datum = p.Datum
            }));
        }

        await context.SaveChangesAsync();     // Pas nu gaan alle toegevoegde rijen echt naar PostgreSQL.
    }

    private static async Task<List<T>> LoadAsync<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
    }
}