const lijst = document.getElementById("productenLijst");

async function toonProducten() {
  try {
    const producten = await haalAllesOp();
    lijst.innerHTML = "";

    producten.forEach((product) => {
      const div = document.createElement("div");
      div.className = "product";

      const titel = document.createElement("h3");
      titel.textContent = product.name;

      const prijs = document.createElement("p");
      prijs.textContent = product.data?.price
        ? `Prijs: €${product.data.price}`
        : "Geen prijs bekend";

      div.append(titel, prijs);
      lijst.appendChild(div);
    });
  } catch (error) {
    lijst.textContent = "Kon de producten niet laden. Probeer het later opnieuw.";
    console.error(error);
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