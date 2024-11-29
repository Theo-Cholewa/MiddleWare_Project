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

          if (startInputField.value && endInputField.value) {
            getRoute(startInputField.value, endInputField.value);
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



function getRoute(startLng, startLat, endLng, endLat) {
  console.log("Call GetRoute");

  coordinates = startLng.ToString(CultureInfo.InvariantCulture) + "," + startLat.ToString(CultureInfo.InvariantCulture) + ";" + endLng.ToString(CultureInfo.InvariantCulture) + "," + endLat.ToString(CultureInfo.InvariantCulture);
  const url = "http://router.project-osrm.org/route/v1/driving/" + coordinates + "?overview=full&steps=true";

  fetch(url)
    .then(response => response.json())
    .then(data => {
      if (data.routes && data.routes.length > 0) {
        const route = data.routes[0];
        console.log("Distance : " + route.distance + " mètres");
        console.log("Durée : " + route.duration + " secondes");

        // Récupérer les coordonnées du trajet
        const routeCoordinates = routes.geometry.coordinates;

        // Dessiner la polyline sur la carte
        drawRoute(routeCoordinates);
      } else {
        console.error("Aucune route trouvée");
      }
    })
    .catch(error => {
      console.error("Erreur lors de la récupération du trajet: ", error);
    })
}

function drawRoute(coordinates) {
  // Supprimer les anciennes polylines si nécessaire
  if (window.routePolyline) {
    map.removeLayer(window.routePolyline);
  }

  // Dessiner la nouvelle polyline
  window.routePolyline = L.polyline(coordinates.map(coord => [coord[1], coord[0]]), { color: 'blue' }).addTo(map);

  // Ajuster la vue sur la carte pour afficher toute la route
  map.fitBounds(window.routePolyline.getBounds());
}