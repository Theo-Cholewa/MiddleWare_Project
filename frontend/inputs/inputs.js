class OriginDestinationInput extends HTMLElement {
    constructor() {
        super();

        let shadowRoot = this.attachShadow({ mode: "open" });

        fetch("inputs/inputs.html").then(async function(response) {
            let htmlContent = await response.text();
            // DOMParser contains methods to create DOM element, parseFromString creates an HTML page with the given content, and then we just have to retrieve the template.
            let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;

            shadowRoot.appendChild(templateContent.cloneNode(true));

            console.log(shadowRoot); // Log shadowRoot content for debugging

            // This event is a customEvent called by autocomplete-adress whenever the user clicks on an option.
            // There is no particular goal here, it's just to show you how to retrieve data from a child component.
            document.addEventListener("optionChosen", function(event) {
                console.log(`selected ${event.detail.name} point: ${event.detail.adress.properties.label}`);
            });

            let button = shadowRoot.querySelector("button");
            button.addEventListener("click", function() {
                let originComponent = shadowRoot.querySelector('auto-complete-input#start');
                let destinationComponent = shadowRoot.querySelector('auto-complete-input#end');
                
                if (originComponent && destinationComponent) {
                    let originValue = originComponent.inputValue;
                    let destinationValue = destinationComponent.inputValue;
                    
                    console.log(`Origin: ${originValue}`);
                    console.log(`Destination: ${destinationValue}`);
                    // http://localhost:8734/Design_Time_Addresses/ServerSide/Service1/ 
                    fetch(`http://localhost:8734/RoutingServer/path?start=${originValue}&end=${destinationValue}`).then( 
                        response => response.json().then(data => {
                            console.log(data);
                        }));
                } else {
                    console.error("Origin or destination component not found!");
                }
            });
        });
    }
}

customElements.define("origin-destination-input", OriginDestinationInput);
