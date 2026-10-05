const blogLijst = document.getElementById("blogLijst");

function parseDatum(datumStr) {
  const [dag, maand, jaar] = datumStr.split("-").map(Number);
  return new Date(jaar, maand - 1, dag);
}

const toonBlogposts = (posts) => {
  blogLijst.innerHTML = "";

  posts.forEach((post, index) => {
    const artikel = document.createElement("div");
    artikel.className = "blogpost";
    artikel.setAttribute("aria-labelledby", `blog-titel-${index}`);

    const titel = document.createElement("h2");
    titel.id = `blog-titel-${index}`;
    titel.textContent = post.titel;

    const [dag, maand, jaar] = post.datum.split("-");
    const datum = document.createElement("time");
    datum.dateTime = `${jaar}-${maand}-${dag}`;
    datum.textContent = post.datum;

    const inhoud = document.createElement("p");
    inhoud.className = "blog-inhoud";
    inhoud.textContent = post.inhoud;

    artikel.append(titel, datum, inhoud);
    blogLijst.appendChild(artikel);
  });
};

fetch("./Blogposts.json")
  .then((response) => response.json())
  .then((data) => {
    const posts = [...data].sort((a, b) => parseDatum(b.datum) - parseDatum(a.datum));
    toonBlogposts(posts);
  })
  .catch((error) => console.error("Kon blogposts niet laden:", error));