const STORAGE_KEY = "mijn-team";

const $ = (id) => {
  const el = document.getElementById(id);
  if (!el) throw new Error(`Element #${id} niet gevonden: controleer het id in je HTML.`);
  return el;
};

const teamLijst = $("teamLijst");
const form = $("productForm");
const formTitel = $("formTitel");
const naamInput = $("naam");
const pokedexInput = $("pokedex");
const statsInput = $("stats");
const typesInput = $("types");
const opslaanKnop = $("opslaanKnop");
const annuleerKnop = $("annuleerKnop");
const statusEl = $("status");

let team = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? "[]");
let bewerkId = null;

function bewaar() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(team));
}

function toonStatus(tekst, isFout = false) {
  statusEl.textContent = tekst;
  statusEl.style.color = isFout ? "red" : "green";
}

function maakKaart(pokemon) {
  const div = document.createElement("div");
  div.className = "pokemon-item";

  const naam = document.createElement("h3");
  naam.textContent = pokemon.naam;

  const pokedex = document.createElement("p");
  pokedex.textContent = pokemon.pokedex ? `Pokedex: ${pokemon.pokedex}` : "";

  const stats = document.createElement("p");
  stats.textContent = pokemon.stats ? `Stats: ${pokemon.stats}` : "";

  const types = document.createElement("p");
  types.textContent = pokemon.types ? `Types: ${pokemon.types}` : "";

  const bewerkKnop = document.createElement("button");
  bewerkKnop.type = "button";
  bewerkKnop.textContent = "Bewerken";
  bewerkKnop.dataset.actie = "bewerk";
  bewerkKnop.dataset.id = pokemon.id;

  const verwijderKnop = document.createElement("button");
  verwijderKnop.type = "button";
  verwijderKnop.textContent = "Verwijderen";
  verwijderKnop.dataset.actie = "verwijder";
  verwijderKnop.dataset.id = pokemon.id;

  div.append(naam, pokedex, stats, types, bewerkKnop, verwijderKnop);
  return div;
}

function toonTeam() {
  teamLijst.innerHTML = "";
  if (team.length === 0) {
    teamLijst.textContent = "Nog geen Pokémon toegevoegd.";
    return;
  }
  team.forEach((p) => teamLijst.appendChild(maakKaart(p)));
}

function resetFormulier() {
  form.reset();
  bewerkId = null;
  formTitel.textContent = "Nieuwe pokemon";
  opslaanKnop.textContent = "Toevoegen";
  annuleerKnop.hidden = true;
}

function startBewerken(pokemon) {
  bewerkId = pokemon.id;
  naamInput.value = pokemon.naam;
  pokedexInput.value = pokemon.pokedex ?? "";
  statsInput.value = pokemon.stats ?? "";
  typesInput.value = pokemon.types ?? "";
  formTitel.textContent = `Pokémon wijzigen`;
  opslaanKnop.textContent = "Opslaan";
  annuleerKnop.hidden = false;
  naamInput.focus();
}

form.addEventListener("submit", (event) => {
  event.preventDefault();

  const payload = {
    naam: naamInput.value.trim(),
    pokedex: pokedexInput.value.trim(),
    stats: statsInput.value.trim(),
    types: typesInput.value.trim(),
  };

  if (!payload.naam) {
    toonStatus("Vul minimaal een naam in.", true);
    return;
  }

  if (bewerkId) {
    team = team.map((p) => (p.id === bewerkId ? { ...payload, id: bewerkId } : p));
    toonStatus("Pokémon gewijzigd.");
  } else {
    team.push({ ...payload, id: crypto.randomUUID() });
    toonStatus("Pokémon toegevoegd.");
  }

  bewaar();
  resetFormulier();
  toonTeam();
});

annuleerKnop.addEventListener("click", resetFormulier);

teamLijst.addEventListener("click", (event) => {
  const knop = event.target.closest("button[data-actie]");
  if (!knop) return;

  const { actie, id } = knop.dataset;
  const pokemon = team.find((p) => p.id === id);
  if (!pokemon) return;

  if (actie === "bewerk") {
    startBewerken(pokemon);
  } else if (actie === "verwijder") {
    if (!confirm(`"${pokemon.naam}" verwijderen uit je team?`)) return;
    team = team.filter((p) => p.id !== id);
    bewaar();
    if (bewerkId === id) resetFormulier();
    toonTeam();
    toonStatus("Pokémon verwijderd.");
  }
});

toonTeam();