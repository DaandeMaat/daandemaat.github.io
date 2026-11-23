using System.Text.Json;
using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace daandemaat.github.io.Data;

public static class DbSeeder
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private record BlogSeed(string Titel, string Inhoud, DateOnly Datum);
    private record ProjectSeed(string Titel, string Beschrijving, string Link, string Categorie, DateOnly Datum);

    public static async Task SeedAsync(AppDbContext context, string contentRootPath)
    {
        var seedPath = Path.Combine(contentRootPath, "Data", "Seed");

        if (!await context.Blogs.AnyAsync())
        {
            var blogs = await LoadAsync<BlogSeed>(Path.Combine(seedPath, "blogs.json"));
            context.Blogs.AddRange(blogs.Select(b => new Blog
            {
                Titel = b.Titel,
                Inhoud = b.Inhoud,
                Datum = b.Datum
            }));
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

        await context.SaveChangesAsync();
    }

    private static async Task<List<T>> LoadAsync<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
    }
    public static async Task SeedUsersAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Gebruiker", "Editor", "Admin" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var testUsers = new[]
        {
            (Email: "admin@voorbeeld.com",     Role: "Admin"),
            (Email: "editor@voorbeeld.com",    Role: "Editor"),
            (Email: "gebruiker@voorbeeld.com", Role: "Gebruiker"),
        };

        foreach (var (email, role) in testUsers)
        {
            if (await userManager.FindByNameAsync(email) is not null) continue;

            var user = new User { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, "Testwachtwoord123!");

            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
            else
                foreach (var error in result.Errors)
                    Console.WriteLine($"[SEED ERROR] {email}: {error.Description}");
        }
    }
}