fetch("../Navbar.html")
  .then(response => response.text())
  .then(html => {
    document.getElementById("navbar").innerHTML = html;
  });