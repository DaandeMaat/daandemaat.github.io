const form = document.querySelector("#contact-form");

const velden = [
    document.querySelector("#naam"), 
    document.querySelector("#email"), 
    document.querySelector("#bericht")];

function valideerVeld(veld) {
    if(veld.value.trim() === "") {
        veld.style.borderColor = "red";

input.setAttribute("aria-invalid", String(!geldig));

        return false;
    }        

form.addEventListener('submit', (event) => {
    event.preventDefault();

    const alleGeldig = velden.map(valideerVeld).every(Boolean);
    const status = document.querySelector("#form-status");

    if(!alleGeldig) {
        status.textContent = "Niet alle velden zijn correct ingevuld.";
        status.style.color = "red";
        return;
    }

    status.textContent = "Formulier succesvol verzonden!";
    form.reset();
    console.log(status);
}