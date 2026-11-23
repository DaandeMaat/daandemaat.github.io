using System.Net.Http.Json;
using daandemaat.github.io.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace daandemaat.github.io.UnitTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string Wachtwoord = "Testwachtwoord123!";
    public const string Admin = "admin@voorbeeld.com";
    public const string Editor = "editor@voorbeeld.com";
    public const string Gebruiker = "gebruiker@voorbeeld.com";

    private readonly string _dbName = "tests-" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_dbName));
        });
    }

    public HttpClient CreateApiClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        HandleCookies = true,
        AllowAutoRedirect = false
    });

    public async Task<HttpClient> LoginAsync(string email, string wachtwoord = Wachtwoord)
    {
        var client = CreateApiClient();
        var response = await client.PostAsJsonAsync("/login?useCookies=true", new { email, password = wachtwoord });
        response.EnsureSuccessStatusCode();
        return client;
    }
}