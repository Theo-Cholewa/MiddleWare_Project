// This class is a container for 2 subcomponents defining origin and destination.
class OriginDestination extends HTMLElement {
    constructor() {
      super();

      let shadowRoot = this.attachShadow({ mode: "open" });

      fetch("origindestination/origin-destination.html").then(async function(response) {
        let htmlContent = await response.text();
        // DOMParser contains methods to create DOM element, parseFromString creates an HTML page with the given content, and then we just have to retrieve the template.
        let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;

        shadowRoot.appendChild(templateContent.cloneNode(true));

        // This event is a customEvent called by autocomplete-adress whenever the user clicks on an option.
        // There is no particular goal here, it's just to show you how to retrieve data from a child component.
        /*document.addEventListener("optionChosen", function(event) {
            console.log(`selected ${event.detail.name} point: ${event.detail.adress.properties.label}`);
            const slot = document.querySelector('slot[name="origin"]');
        });*/

        document.addEventListener("optionChosen", function(event) { 
          const name = event.detail.name; 
          const adress = event.detail.adress; 
          console.log(adress);
          if (name === "start") { 
            const originSlot = document.querySelector('slot[name="origin"]'); 
            console.log("start");
            if (originSlot) { 
              originSlot.innerHTML = `<span>${adress}</span>`; 
            } 
          } else if (name === "end") { 
            console.log("destination");
            const destinationSlot = document.querySelector('slot[name="destination"]'); 
            if (destinationSlot) { 
              destinationSlot.innerHTML = `<span>${adress}</span>`; 
            } 
          } 
          console.log(`selected ${event.detail.name} point: ${event.detail.adress}`); });
      });

    }

    connectedCallback() { // Accéder aux éléments auto-complete-input enfants 
      const autoCompleteInputs = this.querySelectorAll('auto-complete-input[name="start"]'); 
      autoCompleteInputs.forEach(input => { // Exemples d'actions sur les éléments enfants 
        input.addEventListener('input', (e) => { 
          console.log(`Input dans ${input.getAttribute('slot')}: ${e.target.value}`); 
        }); 
      }); 
    }
}

customElements.define("origin-destination", OriginDestination);