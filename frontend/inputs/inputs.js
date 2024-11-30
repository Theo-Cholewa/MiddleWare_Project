class OriginDestinationInput extends HTMLElement {
    constructor() {
        super();
        let shadowRoot = this.attachShadow({ mode: "open" });

        fetch("inputs/inputs.html").then(async function(response) {
            let htmlContent = await response.text();
            let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
            shadowRoot.appendChild(templateContent.cloneNode(true));

            console.log(shadowRoot); // Log shadowRoot content for debugging

            // Sélectionner le bouton et lui ajouter un écouteur d'événement
            let button = shadowRoot.querySelector("button");
            button.addEventListener("click", function() {
                let originComponent = shadowRoot.querySelector('auto-complete-input#start');
                let destinationComponent = shadowRoot.querySelector('auto-complete-input#end');

                if (originComponent && destinationComponent) {
                    let originValue = originComponent.inputValue;
                    let destinationValue = destinationComponent.inputValue;

                    console.log(`Origin: ${originValue}`);
                    console.log(`Destination: ${destinationValue}`);

                    const event = new CustomEvent('origin-destination-changed', {
                        detail: {
                            origin: originValue,
                            destination: destinationValue
                        }
                    });
                    // Émettez l'événement
                    window.dispatchEvent(event);

                    fetch(`http://localhost:8734/Design_Time_Addresses/ServerSide/Service1/path?start=${originValue}&end=${destinationValue}`).then( 
                        response => response.json().then(data => {
                            console.log(data);
                        })
                    );
                } else {
                    console.error("Origin or destination component not found!");
                }
            });
        });
    }
}

customElements.define("origin-destination-input", OriginDestinationInput);
