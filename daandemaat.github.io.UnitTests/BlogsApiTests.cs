using System.Net;
using System.Net.Http.Json;
using daandemaat.github.io.Models;

namespace daandemaat.github.io.UnitTests;

public class BlogsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Url = "/api/blog";

    private static Blog NieuweBlog(string titel = "Testblog") => new()
    {
        Titel = titel,
        Inhoud = "Inhoud van de testblog",
        Datum = new DateOnly(2026, 11, 23)
    };

    private static async Task<Blog> MaakAan(HttpClient admin, string titel = "Testblog")
    {
        var response = await admin.PostAsJsonAsync(Url, NieuweBlog(titel));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Blog>())!;
    }

    [Fact]
    public async Task GetAll_Anoniem_GeeftAangemaakteBlogTerug()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "GetAll-blog");

        var response = await factory.CreateApiClient().GetAsync(Url);
        var lijst = await response.Content.ReadFromJsonAsync<List<Blog>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(lijst!, b => b.Id == blog.Id && b.Titel == "GetAll-blog");
    }

    [Fact]
    public async Task GetById_BestaandId_GeeftJuisteGegevens()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "GetById-blog");

        var response = await factory.CreateApiClient().GetAsync($"{Url}/{blog.Id}");
        var opgehaald = await response.Content.ReadFromJsonAsync<Blog>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(blog.Id, opgehaald!.Id);
        Assert.Equal("GetById-blog", opgehaald.Titel);
        Assert.Equal("Inhoud van de testblog", opgehaald.Inhoud);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        var client = factory.CreateApiClient();

        var response = await client.GetAsync($"{Url}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Admin_Geeft201EnBlogIsOpvraagbaar()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);

        var response = await admin.PostAsJsonAsync(Url, NieuweBlog("Post-blog"));
        var teruggegeven = await response.Content.ReadFromJsonAsync<Blog>();
        var opgehaald = await admin.GetFromJsonAsync<Blog>($"{Url}/{teruggegeven!.Id}");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith($"{Url}/{teruggegeven.Id}", response.Headers.Location!.ToString());
        Assert.Equal("Post-blog", opgehaald!.Titel);
        Assert.Equal("Inhoud van de testblog", opgehaald.Inhoud);
    }

    [Fact]
    public async Task Post_OngeldigeInvoer_Geeft400EnVoegtNietsToe()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var voor = (await admin.GetFromJsonAsync<List<Blog>>(Url))!.Count;

        var response = await admin.PostAsJsonAsync(Url, NieuweBlog(titel: ""));
        var na = (await admin.GetFromJsonAsync<List<Blog>>(Url))!.Count;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(voor, na);
    }

    [Fact]
    public async Task Post_Anoniem_Geeft401()
    {
        var anoniem = factory.CreateApiClient();

        var response = await anoniem.PostAsJsonAsync(Url, NieuweBlog());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_GebruikerZonderRechten_Geeft403()
    {
        var gebruiker = await factory.LoginAsync(ApiFactory.Gebruiker);

        var response = await gebruiker.PostAsJsonAsync(Url, NieuweBlog());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Put_Admin_Geeft204EnWijzigingIsZichtbaar()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "Voor-wijziging");
        var gewijzigd = NieuweBlog("Na-wijziging");
        gewijzigd.Id = blog.Id;

        var response = await admin.PutAsJsonAsync($"{Url}/{blog.Id}", gewijzigd);
        var opgehaald = await admin.GetFromJsonAsync<Blog>($"{Url}/{blog.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("Na-wijziging", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_OnbekendId_Geeft404()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = NieuweBlog();
        blog.Id = 999999;

        var response = await admin.PutAsJsonAsync($"{Url}/999999", blog);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_GebruikerZonderRechten_Geeft403EnLaatBlogOngewijzigd()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "Origineel");
        var gebruiker = await factory.LoginAsync(ApiFactory.Gebruiker);
        var wijziging = NieuweBlog("Geweigerd");
        wijziging.Id = blog.Id;

        var response = await gebruiker.PutAsJsonAsync($"{Url}/{blog.Id}", wijziging);
        var opgehaald = await admin.GetFromJsonAsync<Blog>($"{Url}/{blog.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Origineel", opgehaald!.Titel);
    }

    [Fact]
    public async Task Delete_Admin_Geeft204EnBlogIsVerdwenen()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "Te-verwijderen");

        var response = await admin.DeleteAsync($"{Url}/{blog.Id}");
        var daarna = await admin.GetAsync($"{Url}/{blog.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, daarna.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);

        var response = await admin.DeleteAsync($"{Url}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Anoniem_Geeft401EnBlogBlijftBestaan()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var blog = await MaakAan(admin, "Blijft-bestaan");

        var response = await factory.CreateApiClient().DeleteAsync($"{Url}/{blog.Id}");
        var daarna = await admin.GetAsync($"{Url}/{blog.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, daarna.StatusCode);
    }
}