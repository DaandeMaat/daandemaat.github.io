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

    public static async Task SeedAsync(AppDbContext context, string contentRootPath)
    {
        var seedPath = Path.Combine(contentRootPath, "Data", "Seed");

        if (!await context.Blogs.AnyAsync())
        {
            var blogs = await LoadAsync<Blog>(Path.Combine(seedPath, "Blogposts.json"));
            foreach (var blog in blogs) blog.Id = 0;
            context.Blogs.AddRange(blogs);
        }

        if (!await context.Projecten.AnyAsync())
        {
            var projecten = await LoadAsync<Project>(Path.Combine(seedPath, "ProjectDatum.json"));
            foreach (var project in projecten) project.Id = 0;
            context.Projecten.AddRange(projecten);
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<T>> LoadAsync<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
    }
}