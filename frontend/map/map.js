var map = L.map('map').setView([51.505, -0.09], 13);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
  maxZoom: 19,
  attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

map.on('click', function(e) {
  var lat = e.latlng.lat;  // Latitude
  var lng = e.latlng.lng;  // Longitude
  
  console.log("Coordonnées du clic : Latitude = " + lat + ", Longitude = " + lng);

  // Sélectionner les éléments auto-complete-input
  const startInput = document.querySelector('auto-complete-input[slot="origin"]');
  const endInput = document.querySelector('auto-complete-input[slot="destination"]');

  // Vérifier si l'un des deux champs est vide
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
