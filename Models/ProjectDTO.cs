namespace daandemaat.github.io.Models;

public class Project
{
public int Id { get; set; }
public string Titel { get; set; } = string.Empty;
public string Beschrijving { get; set; } = string.Empty;
public string Categorie { get; set; } = string.Empty;
public string Link { get; set; } = string.Empty;
public DateOnly Datum { get; set; }
}
