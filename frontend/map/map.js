var map = L.map('map').setView([51.505, -0.09], 13);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
  maxZoom: 19,
  attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

map.on('click', function(e) {
  var lat = e.latlng.lat;  // Latitude
  var lng = e.latlng.lng;  // Longitude
  
  console.log("Coordonnées du clic : Latitude = " + lat + ", Longitude = " + lng);

  // Sélectionner l'élément origin-destination-input
  const originDestinationInput = document.querySelector('origin-destination-input');
  const startInput = originDestinationInput.shadowRoot.querySelector('#start');
  const endInput = originDestinationInput.shadowRoot.querySelector('#end');

  // Vérifier si l'un des champs est vide
  const startInputField = startInput ? startInput.shadowRoot.querySelector('input') : null;
  const endInputField = endInput ? endInput.shadowRoot.querySelector('input') : null;

  if ((!startInputField || !startInputField.value) || (!endInputField || !endInputField.value)) {
    // Si l'un des champs est vide, faire la requête de géocodage inverse
    var url = `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lng}&format=json`;

    fetch(url)
      .then(response => response.json())
      .then(data => {
        if (data && data.address) {
          var address = data.address;
          var fullAddress = `${address.road || ''}, ${address.city || ''}, ${address.country || ''}`.trim();
          console.log("Adresse trouvée : " + fullAddress);

          // Si le premier champ est vide, le remplir
          if (startInputField && !startInputField.value) {
            startInputField.value = fullAddress;
          }
          // Si le deuxième champ est vide, le remplir
          else if (endInputField && !endInputField.value) {
            endInputField.value = fullAddress;
          }
        }
        else {
          console.log("Aucune adresse trouvée");
        }
      })
      .catch(error => {
        console.error("Erreur lors du géocodage inverse:", error);
      });
  }
});

function draw(startAddress, endAddress) {
  let lngStart, latStart, lngEnd, latEnd;
  Promise.all([
    getCoordinates(startAddress),
    getCoordinates(endAddress)
  ])
  .then(([startCoord, endCoord]) => {
    lngStart = startCoord.longitude;
    latStart = startCoord.latitude;
    lngEnd = endCoord.longitude;
    latEnd = endCoord.latitude;

    console.log("Start, longitude: " + lngStart + ", latitude: " + latStart);
    console.log("End, longitude: " + lngEnd + ", latitude: " + latEnd);

    getRoute(lngStart, latStart, lngEnd, latEnd);
  })
  .catch(error => {
    console.error("Error fetching coordinates:", error);
  })
}

// Fonction qui prend une adresse et retourne une promesse avec la latitude et la longitude
function getCoordinates(address) {
  return new Promise((resolve, reject) => {
    const url = `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&limit=1`;

    // Effectuer une requête HTTP GET vers l'API Nominatim
    fetch(url)
      .then(response => {
        if (!response.ok) {
          reject('Erreur lors de la récupération des données.');
        }
        return response.json();
      })
      .then(data => {
        if (data.length > 0) {
          // On récupère les coordonnées du premier résultat
          const lat = data[0].lat;
          const lon = data[0].lon;
          resolve({ latitude: lat, longitude: lon });
        } else {
          reject('Aucun résultat trouvé pour cette adresse.');
        }
      })
      .catch(error => {
        reject('Erreur de connexion: ' + error);
      });
  });
}


function getRoute(startLng, startLat, endLng, endLat) {
  
  const apiKey = "5b3ce3597851110001cf624890fd5d77ab2949f7b0b6d7b9ba15d1df";
  const url = `https://api.openrouteservice.org/v2/directions/driving-car?api_key=${apiKey}&start=${startLng},${startLat}&end=${endLng},${endLat}`;

  clearMap()

  fetch(url)
        .then(response => response.json())
        .then(data => {
            if (data && data.features) {
                const coords = data.features[0].geometry.coordinates;

                // Convertir les coordonnées GeoJSON en Leaflet (LatLng)
                const latLngs = coords.map(coord => [coord[1], coord[0]]);

                // Ajouter une polyligne à la carte
                const routeLine = L.polyline(latLngs, { color: 'blue', weight: 4 }).addTo(map);

                // Ajuster la vue pour inclure le trajet
                map.fitBounds(routeLine.getBounds());
            } else {
                console.error("Aucun itinéraire trouvé.");
            }
        })
        .catch(error => console.error("Erreur lors de la récupération de l'itinéraire :", error));
}

function clearMap() {
  map.eachLayer(function(layer) {
    if (layer instanceof L.Polyline) {
      map.removeLayer(layer);
    }
  });
}

window.addEventListener('origin-destination-changed', function(event) {
  const { origin, destination } = event.detail;
  console.log(`Received origin: ${origin}, destination: ${destination}`);

  // Appelez la fonction draw avec les paramètres appropriés
  draw(origin, destination);
});