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

function getRoute(coords, col) {
  clearMap()

  let currentColour = 'blue';
  let colours = ['blue'];
  for (let i = 1; i < coords.length; i++) {
    if (col.includes(i)) {
      currentColour = currentColour === "blue" ? "red" : "blue";
    }
    colours.push(currentColour);
  }

  const latLngs = coords.map(coord => [parseFloat(coord[0]), parseFloat(coord[1])]);

  const polylineSegments = [];

  for (let i = 0; i < latLngs.length - 1; i++) {
    const segment = [latLngs[i], latLngs[i + 1]];
    const colour = colours[i];

    L.polyline(segment, { color: colour, weight: 4 }).addTo(map);

    polylineSegments.push(segment);
  }

  const bounds = L.latLngBounds(polylineSegments.flat());
  map.fitBounds(bounds);
}


function clearMap() {
  map.eachLayer(function(layer) {
    if (layer instanceof L.Polyline) {
      map.removeLayer(layer);
    }
  });
}

window.addEventListener('pathUpdated', function (event) {
  const { coord, col } = event.detail;
  console.log('Coords: ' + JSON.stringify(coord));  // Affiche correctement le tableau de tableaux
  console.log('colours: ' + col);

  getRoute(coord, col);
})