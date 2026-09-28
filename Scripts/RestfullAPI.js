const BASE_URL = "https://api.restful-api.dev/objects";

async function apiRequest(url, options = {}) {
  const response = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    throw new Error(`Request mislukt: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

const haalAllesOp = () => apiRequest(BASE_URL);

const haalEenOp = (id) => apiRequest(`${BASE_URL}/${id}`);

const wijzig = (id, wijzigingen) =>
  apiRequest(`${BASE_URL}/${id}`, { method: "PUT", body: JSON.stringify(wijzigingen) });

const voegToe = (product) =>
  apiRequest(BASE_URL, { method: "POST", body: JSON.stringify(product) });

const verwijder = (id) => apiRequest(`${BASE_URL}/${id}`, { method: "DELETE" });