using System.Net;
using System.Net.Http.Json;
using daandemaat.github.io.Models;

namespace daandemaat.github.io.UnitTests;

public class ProjectenApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Url = "/api/project";

    private static Project NieuwProject(string titel = "Testproject") => new()
    {
        Titel = titel,
        Beschrijving = "Beschrijving van het testproject",
        Link = "https://example.com/test",
        Categorie = "Web",
        Datum = new DateOnly(2026, 11, 23)
    };

    private static async Task<Project> MaakAan(HttpClient admin, string titel = "Testproject")
    {
        var response = await admin.PostAsJsonAsync(Url, NieuwProject(titel));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Project>())!;
    }

    [Fact]
    public async Task GetAll_AnoniemMagLezen_GeeftAangemaaktProjectTerug()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var aangemaakt = await MaakAan(admin, "GetAll-project");
        var anoniem = factory.CreateApiClient();

        var response = await anoniem.GetAsync(Url);
        var lijst = await response.Content.ReadFromJsonAsync<List<Project>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(lijst!, p => p.Id == aangemaakt.Id && p.Titel == "GetAll-project");
    }

    [Fact]
    public async Task GetById_BestaandId_GeeftJuisteGegevens()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var aangemaakt = await MaakAan(admin, "GetById-project");

        var response = await factory.CreateApiClient().GetAsync($"{Url}/{aangemaakt.Id}");
        var project = await response.Content.ReadFromJsonAsync<Project>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(aangemaakt.Id, project!.Id);
        Assert.Equal("GetById-project", project.Titel);
        Assert.Equal("Web", project.Categorie);
        Assert.Equal(new DateOnly(2026, 11, 23), project.Datum);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        var client = factory.CreateApiClient();

        var response = await client.GetAsync($"{Url}/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_AdminMetGeldigeInvoer_Geeft201EnProjectIsOpvraagbaar()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var nieuw = NieuwProject("Post-project");

        var response = await admin.PostAsJsonAsync(Url, nieuw);
        var teruggegeven = await response.Content.ReadFromJsonAsync<Project>();
        var opgehaald = await admin.GetFromJsonAsync<Project>($"{Url}/{teruggegeven!.Id}");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.EndsWith($"{Url}/{teruggegeven.Id}", response.Headers.Location!.ToString());
        Assert.True(teruggegeven.Id > 0);
        Assert.Equal("Post-project", teruggegeven.Titel);
        Assert.Equal("Post-project", opgehaald!.Titel);
        Assert.Equal(nieuw.Link, opgehaald.Link);
    }

    [Fact]
    public async Task Post_AdminMetOngeldigeInvoer_Geeft400EnVoegtNietsToe()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var voor = (await admin.GetFromJsonAsync<List<Project>>(Url))!.Count;
        var ongeldig = NieuwProject(titel: "");

        var response = await admin.PostAsJsonAsync(Url, ongeldig);
        var na = (await admin.GetFromJsonAsync<List<Project>>(Url))!.Count;

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(voor, na);
    }

    [Fact]
    public async Task Post_Anoniem_Geeft401EnVoegtNietsToe()
    {
        var anoniem = factory.CreateApiClient();
        var voor = (await anoniem.GetFromJsonAsync<List<Project>>(Url))!.Count;

        var response = await anoniem.PostAsJsonAsync(Url, NieuwProject("Anoniem"));
        var na = (await anoniem.GetFromJsonAsync<List<Project>>(Url))!.Count;

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(voor, na);
    }

    [Theory]
    [InlineData(ApiFactory.Editor)]
    [InlineData(ApiFactory.Gebruiker)]
    public async Task Post_IngelogdZonderAdminRol_Geeft403EnVoegtNietsToe(string email)
    {
        var client = await factory.LoginAsync(email);
        var voor = (await client.GetFromJsonAsync<List<Project>>(Url))!.Count;

        var response = await client.PostAsJsonAsync(Url, NieuwProject("Geweigerd"));
        var na = (await client.GetFromJsonAsync<List<Project>>(Url))!.Count;

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(voor, na);
    }

    [Theory]
    [InlineData(ApiFactory.Editor)]
    [InlineData(ApiFactory.Admin)]
    public async Task Put_EditorEnAdmin_Geeft204EnWijzigingIsZichtbaar(string email)
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Voor-wijziging");
        var schrijver = await factory.LoginAsync(email);
        var gewijzigd = NieuwProject($"Gewijzigd door {email}");
        gewijzigd.Id = project.Id;

        var response = await schrijver.PutAsJsonAsync($"{Url}/{project.Id}", gewijzigd);
        var opgehaald = await admin.GetFromJsonAsync<Project>($"{Url}/{project.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal($"Gewijzigd door {email}", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_OnbekendId_Geeft404()
    {
        var editor = await factory.LoginAsync(ApiFactory.Editor);
        var project = NieuwProject();
        project.Id = 999999;

        var response = await editor.PutAsJsonAsync($"{Url}/999999", project);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_OngeldigeInvoer_Geeft400EnLaatProjectOngewijzigd()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Blijft-ongewijzigd");
        var ongeldig = NieuwProject(titel: "");
        ongeldig.Id = project.Id;

        var response = await admin.PutAsJsonAsync($"{Url}/{project.Id}", ongeldig);
        var opgehaald = await admin.GetFromJsonAsync<Project>($"{Url}/{project.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Blijft-ongewijzigd", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_Anoniem_Geeft401EnLaatProjectOngewijzigd()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Origineel");
        var wijziging = NieuwProject("Anoniem gewijzigd");
        wijziging.Id = project.Id;

        var response = await factory.CreateApiClient().PutAsJsonAsync($"{Url}/{project.Id}", wijziging);
        var opgehaald = await admin.GetFromJsonAsync<Project>($"{Url}/{project.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Origineel", opgehaald!.Titel);
    }

    [Fact]
    public async Task Put_GebruikerZonderRechten_Geeft403EnLaatProjectOngewijzigd()
    {
        // Arrange
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Origineel-403");
        var gebruiker = await factory.LoginAsync(ApiFactory.Gebruiker);
        var wijziging = NieuwProject("Gebruiker gewijzigd");
        wijziging.Id = project.Id;

        // Act
        var response = await gebruiker.PutAsJsonAsync($"{Url}/{project.Id}", wijziging);
        var opgehaald = await admin.GetFromJsonAsync<Project>($"{Url}/{project.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Origineel-403", opgehaald!.Titel);
    }

    [Fact]
    public async Task Delete_Admin_Geeft204EnProjectIsVerdwenen()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Te-verwijderen");

        var response = await admin.DeleteAsync($"{Url}/{project.Id}");
        var daarna = await admin.GetAsync($"{Url}/{project.Id}");

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
    public async Task Delete_Anoniem_Geeft401EnProjectBlijftBestaan()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Blijft-bestaan-401");

        var response = await factory.CreateApiClient().DeleteAsync($"{Url}/{project.Id}");
        var daarna = await admin.GetAsync($"{Url}/{project.Id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, daarna.StatusCode);
    }

    [Fact]
    public async Task Delete_Editor_Geeft403EnProjectBlijftBestaan()
    {
        var admin = await factory.LoginAsync(ApiFactory.Admin);
        var project = await MaakAan(admin, "Blijft-bestaan-403");
        var editor = await factory.LoginAsync(ApiFactory.Editor);

        var response = await editor.DeleteAsync($"{Url}/{project.Id}");
        var daarna = await admin.GetAsync($"{Url}/{project.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, daarna.StatusCode);
    }
}