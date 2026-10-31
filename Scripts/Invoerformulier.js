const form = document.querySelector("#contact-form");
const statusEl = document.querySelector("#form-status");
const naam = document.querySelector("#naam");
const email = document.querySelector("#email");
const bericht = document.querySelector("#bericht");

if (!form || !statusEl || !naam || !email || !bericht) {
    throw new Error("Formulier-elementen niet gevonden: controleer de id's in de HTML.");
}

const velden = [naam, email, bericht];

const VELD_LABELS = {
    naam: "Naam",
    email: "Email",
    bericht: "Bericht",
};

const NAAM_REGEX = /^[^\d]+$/;
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;  

function berekenFout(veld) {
    const waarde = veld.value.trim();

    if (waarde === "") {
        return "Dit veld is verplicht.";
    }
    if (veld === naam && !NAAM_REGEX.test(waarde)) {
        return "Naam mag geen cijfers bevatten. Je heet niet R2-D2, toch?";
    }
    if (veld === email && !EMAIL_REGEX.test(waarde)) {
        return "Vul een geldig e-mailadres in met een @-teken.";
    }
    return "";
}

function toonFout(veld, tekst) {
    const foutEl = document.getElementById(`${veld.id}-fout`);
    veld.setAttribute("aria-invalid", String(tekst !== ""));
    if (foutEl) {
        foutEl.textContent = tekst;
        foutEl.hidden = tekst === "";
    }
}

velden.forEach((veld) => {
    veld.addEventListener("input", () => {
        if (veld.getAttribute("aria-invalid") === "true") {
            toonFout(veld, berekenFout(veld));
        }
    });
});

form.addEventListener("submit", (event) => {
    event.preventDefault();

    const ongeldig = velden.filter((veld) => {
        const tekst = berekenFout(veld);
        toonFout(veld, tekst);
        return tekst !== "";
    });

    if (ongeldig.length > 0) {
        const namen = ongeldig.map((veld) => VELD_LABELS[veld.id] ?? veld.id).join(", ");
        statusEl.textContent =  `Er ${ongeldig.length === 1 ? "ontbreekt 1 veld" : `ontbreken ${ongeldig.length} velden`}: ${namen}.`,
        statusEl.classList.add("status-fout");
        statusEl.classList.remove("status-succes");
        ongeldig[0].focus();
        return;
    }

    statusEl.textContent = "Formulier succesvol verzonden!";
    statusEl.classList.add("status-succes");
    statusEl.classList.remove("status-fout");
    form.reset();
});