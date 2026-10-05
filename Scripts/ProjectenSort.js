document.addEventListener("DOMContentLoaded", () => {
  let projecten = [];
  let sorteerOplopend = true;

  const projectenLijst = document.getElementById("projectenLijst");
  const sortButton = document.getElementById("changeContentButton");
  const sorteerStatus = document.getElementById("sorteerStatus");

  function parseDatum(datumStr) {
    const [dag, maand, jaar] = datumStr.split("-").map(Number);
    return new Date(jaar, maand - 1, dag);
  }

  function sortProjectenByDatumNieuwNaarOud(projecten) {
    projecten.sort((a, b) => parseDatum(b.datum) - parseDatum(a.datum));
  }

  function sortProjectenByDatumOudNaarNieuw(projecten) {
    projecten.sort((a, b) => parseDatum(a.datum) - parseDatum(b.datum));
  }

  const toonProjecten = (projecten) => {
    projectenLijst.innerHTML = "";

    projecten.forEach((project) => {
      const projectDiv = document.createElement("div");
      projectDiv.className = "project";

      const titel = document.createElement("h3");
      titel.textContent = project.titel;

      const beschrijving = document.createElement("p");
      beschrijving.textContent = project.beschrijving;

      const datum = document.createElement("p");
      datum.textContent = `Gestart op: ${project.datum}`;

      const link = document.createElement("a");
      link.href = project.link;
      link.textContent = "Bekijk op GitHub";
      link.target = "_blank";
      link.setAttribute('aria-label', `Bekijk ${project.titel} op GitHub`);

      projectDiv.appendChild(titel);
      projectDiv.appendChild(beschrijving);
      projectDiv.appendChild(datum);
      projectDiv.appendChild(link);

      projectenLijst.appendChild(projectDiv);
    });
  };

  sortButton.addEventListener("click", () => {
    if (sorteerOplopend) {
      sortProjectenByDatumOudNaarNieuw(projecten);
      sorteerStatus.textContent = "Projecten gesoorteerd van oud naar nieuw";
    } else {
      sortProjectenByDatumNieuwNaarOud(projecten);
      sorteerStatus.textContent = "Projecten gesoorteerd van nieuw naar oud";
    }
    sorteerOplopend = !sorteerOplopend;
    toonProjecten(projecten);
  });

  fetch("./ProjectDatum.json")
    .then((response) => response.json())
    .then((data) => {
      projecten = data;
      toonProjecten(projecten);
    })
    .catch((error) => console.error("Kon projecten niet laden:", error));
});
