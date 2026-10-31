const STORAGE_KEY = "mijn-team";

const $ = (id) => {
  const el = document.getElementById(id);
  if (!el)
    throw new Error(
      `Element #${id} niet gevonden: controleer het id in je HTML.`,
    );
  return el;
};

const teamLijst = $("teamLijst");
const form = $("pokemonForm");
const formTitel = $("formTitel");
const naamInput = $("naam");
const pokedexInput = $("pokedex");
const statsInput = $("stats");
const typesInput = $("types");
const opslaanKnop = $("opslaanKnop");
const annuleerKnop = $("annuleerKnop");
const statusEl = $("status");

const VELD_LABELS = {
  naam: "Naam",
  pokedex: "Pokedex-nummer",
  stats: "Stats",
  types: "Type(s)",
};

const formVelden = [naamInput, pokedexInput, statsInput, typesInput];


const NL_TYPE_VERTALING = {
  normaal: "normal", vuur: "fire", water: "water", elektrisch: "electric",
  gras: "grass", ijs: "ice", vechten: "fighting", gevecht: "fighting",
  gif: "poison", grond: "ground", vliegend: "flying", psychisch: "psychic",
  insect: "bug", steen: "rock", gesteente: "rock", geest: "ghost",
  draak: "dragon", duister: "dark", staal: "steel", fee: "fairy",
};

function naarTypeKleur(naam) {
  const schoon = naam.toLowerCase().trim();
  const engelsNaam = TYPE_KLEUREN[schoon] ? schoon : NL_TYPE_VERTALING[schoon];
  return engelsNaam ? TYPE_KLEUREN[engelsNaam] : null;
}

function zetAchtergrond(div, typeNamen) {
  const kleuren = typeNamen.map(naarTypeKleur).filter(Boolean);

  if (kleuren.length === 0) {
    return;
  }
  div.style.background =
    kleuren.length === 1 ? kleuren[0] : `linear-gradient(135deg, ${kleuren.join(", ")})`;
}

let team = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? "[]");
let bewerkId = null;

function bewaar() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(team));
}

function toonStatus(tekst, isFout = false) {
  statusEl.textContent = tekst;
  statusEl.classList.toggle("status-fout", isFout);
  statusEl.classList.toggle("status-succes", !isFout);
}

function berekenPokemonFout(input) {
  if (input.value.trim() !== "") {
    return "";
  }

  switch (input.id) {
    case "naam":
      return `Vul een naam in voor ${VELD_LABELS[input.id]}.`;
    case "pokedex":
      return `Vul een pokedex nummer in voor ${VELD_LABELS[input.id]}.`;
    case "stats":
      return `Vul een stats waarde in voor ${VELD_LABELS[input.id]}.`;
    case "types":
      return `Geef je pokémon een of twee types voor ${VELD_LABELS[input.id]}.`;
    default:
      return `Het veld "${VELD_LABELS[input.id]}" is verplicht.`;
  }
}

function toonPokemonFout(input, tekst) {
  const foutEl = document.getElementById(`${input.id}-fout`);
  input.setAttribute("aria-invalid", String(tekst !== ""));
  if (foutEl) {
    foutEl.textContent = tekst;
    foutEl.hidden = tekst === "";
  }
}

formVelden.forEach((input) => {
  input.addEventListener("input", () => {
    if (input.getAttribute("aria-invalid") === "true") {
      toonPokemonFout(input, berekenPokemonFout(input));
    }
  });
});

function maakKaart(pokemon) {
  const div = document.createElement("div");
  div.className = "pokemon-item";

  const typeNamen = pokemon.types
    ? pokemon.types.split(",").map((t) => t.trim()).filter(Boolean)
    : [];
  zetAchtergrond(div, typeNamen);

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
  formVelden.forEach((input) => toonPokemonFout(input, ""));
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

  const ongeldig = formVelden.filter((input) => {
    const tekst = berekenPokemonFout(input);
    toonPokemonFout(input, tekst);
    return tekst !== "";
  });

  if (ongeldig.length > 0) {
    const namen = ongeldig.map((input) => VELD_LABELS[input.id]).join(", ");
    toonStatus(
      `Er ${ongeldig.length === 1 ? "ontbreekt 1 veld" : `ontbreken ${ongeldig.length} velden`}: ${namen}.`,
      true,
    );
    ongeldig[0].focus();
    return;
  }

  const payload = {
    naam: naamInput.value.trim(),
    pokedex: pokedexInput.value.trim(),
    stats: statsInput.value.trim(),
    types: typesInput.value.trim(),
  };

  if (bewerkId) {
    team = team.map((p) =>
      p.id === bewerkId ? { ...payload, id: bewerkId } : p,
    );
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