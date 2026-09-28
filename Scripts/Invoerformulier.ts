const form = document.querySelector<HTMLFormElement>("#contact-form");
const statusEl = document.querySelector<HTMLElement>("#form-status");
const naam = document.querySelector<HTMLInputElement>("#naam");
const email = document.querySelector<HTMLInputElement>("#email");
const bericht = document.querySelector<HTMLTextAreaElement>("#bericht");

if (!form || !statusEl || !naam || !email || !bericht) {
  throw new Error(
    "Formulier-elementen niet gevonden: controleer de id's in de HTML.",
  );
}

type Veld = HTMLInputElement | HTMLTextAreaElement;
const velden: Veld[] = [naam, email, bericht];

const valideerVeld = (veld: Veld): boolean => {
  const waarde = veld.value.trim();
  let geldig = waarde !== "";

  if (geldig && veld === email) {
    geldig = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(waarde);
  }

  veld.style.borderColor = geldig ? "" : "red";
  veld.setAttribute("aria-invalid", String(!geldig));
  return geldig;
};

form.addEventListener("submit", (event: SubmitEvent) => {
  event.preventDefault();

  const alleGeldig = velden.map(valideerVeld).every(Boolean);

  if (!alleGeldig) {
    statusEl.textContent = "Niet alle velden zijn correct ingevuld.";
    statusEl.style.color = "red";
    return;
  }

  statusEl.textContent = "Formulier succesvol verzonden!";
  statusEl.style.color = "green";
  form.reset();
});
