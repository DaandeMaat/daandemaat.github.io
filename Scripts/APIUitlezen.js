const lijst = document.getElementById("pokemonLijst");
if (!lijst) {
  throw new Error('Element #pokemonLijst niet gevonden: controleer het id in je HTML.');
}

async function toonPokemons() {
  try {
    const pokemons = await haalAllesOp();
    lijst.innerHTML = "";

    pokemons.forEach((pokemon) => {
      const div = document.createElement("div");
      div.className = "pokemon-item";

      const naam = document.createElement("h3");
      naam.textContent = `#${pokemon.id} ${pokemon.name}`;

      const afbeelding = document.createElement("img");
      afbeelding.src = pokemon.sprites?.front_default ?? "";
      afbeelding.alt = pokemon.name;
      afbeelding.width = 96;

      const types = document.createElement("p");
      types.textContent = `Types: ${pokemon.types.map((t) => t.type.name).join(", ")}`;

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