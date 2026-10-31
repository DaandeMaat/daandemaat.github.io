const lijst = document.getElementById("pokemonLijst");
if (!lijst) {
  throw new Error('Element #pokemonLijst niet gevonden: controleer het id in je HTML.');
}

const TYPE_KLEUREN = {
  normal: "#A8A878", fire: "#F08030", water: "#6890F0", electric: "#F8D030",
  grass: "#78C850", ice: "#98D8D8", fighting: "#C03028", poison: "#A040A0",
  ground: "#E0C068", flying: "#A890F0", psychic: "#F85888", bug: "#A8B820",
  rock: "#B8A038", ghost: "#705898", dragon: "#7038F8", dark: "#705848",
  steel: "#B8B8D0", fairy: "#EE99AC",
};

function zetAchtergrond(div, typeNamen) {
  const kleuren = typeNamen
    .map((naam) => TYPE_KLEUREN[naam.toLowerCase().trim()])
    .filter(Boolean);

  if (kleuren.length === 0) {
    return;
  }
  div.style.background =
    kleuren.length === 1 ? kleuren[0] : `linear-gradient(135deg, ${kleuren.join(", ")})`;
}

async function toonPokemons() {
  try {
    const pokemons = await haalAllesOp();
    lijst.innerHTML = "";

    pokemons.forEach((pokemon) => {
      const div = document.createElement("div");
      div.className = "pokemon-item";

      const typeNamen = pokemon.types.map((t) => t.type.name);
      zetAchtergrond(div, typeNamen);

      const naam = document.createElement("h3");
      naam.textContent = `#${pokemon.id} ${pokemon.name}`;

      const afbeelding = document.createElement("img");
      afbeelding.src = pokemon.sprites?.front_default ?? "";
      afbeelding.alt = pokemon.name;
      afbeelding.width = 98;
      afbeelding.height = 98;

      const types = document.createElement("p");
      types.textContent = `Types: ${typeNamen.join(", ")}`;

      const stats = document.createElement("p");
      stats.textContent = `Stats: ${pokemon.stats
        .map((s) => `${s.stat.name} ${s.base_stat}`)
        .join(", ")}`;

      div.append(naam, afbeelding, types, stats);
      lijst.appendChild(div);
    });
  } catch (error) {
    console.error(error);
    lijst.textContent = "Kon de Pokémon niet laden. Probeer het later opnieuw.";
  }
}

toonPokemons();