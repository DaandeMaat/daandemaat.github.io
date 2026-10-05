const LIST_URL = "https://pokeapi.co/api/v2/pokemon";

async function apiRequest(url) {
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Request mislukt: ${response.status} ${response.statusText}`);
  }
  return response.json();
}

async function haalAllesOp() {
  const lijstData = await apiRequest(LIST_URL);
  return Promise.all(lijstData.results.map((p) => apiRequest(p.url)));
}

const haalEenOp = (naamOfId) =>
  apiRequest(`https://pokeapi.co/api/v2/pokemon/${naamOfId}`);