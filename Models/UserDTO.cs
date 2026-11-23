using Microsoft.AspNetCore.Identity;

namespace daandemaat.github.io.Models;

public class User : IdentityUser
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Wachtwoord { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;

}