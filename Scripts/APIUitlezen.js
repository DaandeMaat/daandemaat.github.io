const lijst = document.getElementById("productenLijst");
if (!lijst) {
  throw new Error('Element #productenLijst niet gevonden: controleer het id in je HTML.');
}

async function toonProducten() {
  try {
    const producten = await haalAllesOp();
    lijst.innerHTML = "";

    producten.forEach((product) => {
      const div = document.createElement("div");
      div.className = "product";

      const titel = document.createElement("h3");
      titel.textContent = product.name;

      const generatie = document.createElement("p")
      generatie.textContent = product.data?.generation
        ? `Generatie: ${product.data.generation}`
        : "Generatie onbekend";

      const prijs = document.createElement("p");
      prijs.textContent = product.data?.price
        ? `Prijs: €${product.data.price}`
        : "Geen prijs bekend";

      const capaciteit = document.createElement("p");
      capaciteit.textContent = product.data?.capacity
        ? `Capaciteit: ${product.data.capacity}`
        : "Capaciteit onbekend";

      div.append(titel, generatie, prijs, capaciteit);
      lijst.appendChild(div);
    });
  } catch (error) {
    console.error(error);
    lijst.textContent = "Kon de producten niet laden. Probeer het later opnieuw.";
  }
}

async function maakLaptopAan() {
  try {
    const nieuw = await voegToe({
      name: "Laptop",
      data: { price: 999, brand: "Example" },
    });
    console.log("Aangemaakt met id:", nieuw.id);
  } catch (error) {
    console.error(error);
  }
}

toonProducten();