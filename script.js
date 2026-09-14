function sorteerOpRelevantie(projectenLijst) {
  projectenLijst.sort((a, b) => b.date - a.date);
}

const projectenLijst = document.getElementById("projectenLijst");
const button = document.getElementById("changeContentButton");
button.addEventListener("click", () => {
  sorteerOpRelevantie(projectenLijst);
  toonProducten(projectenLijst);
});

