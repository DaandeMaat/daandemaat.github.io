namespace daandemaat.github.io.Models;

public class Blog
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateOnly Datum { get; set; }
}
