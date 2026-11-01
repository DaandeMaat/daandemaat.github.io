# daandemaat.github.io

## De API uitproberen met Swagger

Swagger is een pagina in je browser waarmee je de gegevens van deze website (blogs en projecten) kunt opvragen, zonder dat je hoeft te programmeren. Je klikt gewoon op knoppen.

### 1. De applicatie starten

Open een terminal (menu **Terminal → New Terminal**). Typ het volgende commando en druk op Enter:

   dotnet run

Wacht tot je een regel ziet als `Now listening on: https://localhost:5409`. Het nummer achter `localhost:` kan bij jou anders zijn. Dat is normaal.

### 2. Swagger openen

Open je browser en ga naar het adres uit de terminal, met `/swagger` erachter. Bijvoorbeeld:

https://localhost:5409/swagger

Je ziet nu een overzicht van alle beschikbare data, zoals **Blog** en **Projecten**.

### 3. Gegevens opvragen

Voorbeeld: alle blogs bekijken.

Klik op **GET /api/blog**.
Klik rechts op **Try it out**.
Klik op de blauwe knop **Execute**.
Scroll naar beneden. Onder **Response body** staan nu alle blogs.

Voor één specifieke blog:

Klik op **GET /api/blog/{id}** en dan op **Try it out**.
Vul bij **id** een nummer in, bijvoorbeeld `1`.
Klik op **Execute**.

Voor de projecten werkt het precies hetzelfde via **/api/projecten**.

### Wat betekenen de kleuren en meldingen?

| Kleur / code | Betekenis |
| Blauw (GET) | Gegevens opvragen |
| Groen (POST) | Nieuwe gegevens toevoegen |
| Oranje (PUT) | Bestaande gegevens aanpassen |
| Rood (DELETE) | Gegevens verwijderen |
| Code **200** | Gelukt |
| Code **201** | Succesvol aangemaakt |
| Code **404** | Niet gevonden (bijvoorbeeld een id dat niet bestaat) |
| Code **400** | Er klopt iets niet aan wat je hebt ingevuld |

### Gegevens toevoegen, aanpassen of verwijderen

- **Toevoegen:** klik op **POST**, dan **Try it out**, vul de velden in en klik op **Execute**. Een datum schrijf je als `2026-11-01` (jaar-maand-dag). Het id hoef je niet in te vullen, dat wordt automatisch gekozen.
- **Aanpassen (PUT):** vul bij **id** het nummer in van wat je wilt wijzigen, en pas de tekst hieronder aan.
- **Verwijderen (DELETE):** vul het id in en klik op **Execute**. Dit kan niet ongedaan worden gemaakt.