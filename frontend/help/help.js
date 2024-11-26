class Help extends HTMLElement {

    active = false;
    
    constructor() {
        super();
    
        let shadowRoot = this.attachShadow({mode: "open"});

        fetch("/help/help.html")
            .then(response => {
                if (!response.ok) {
                    throw new Error('Échec du chargement du fichier HTML');
                }
                return response.text();
            })
            .then(htmlContent => {
                let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
                shadowRoot.appendChild(templateContent.cloneNode(true));
                this.setupLogic(); // Appeler la fonction pour configurer la logique du composant
            })
            .catch(error => {
                console.error("Erreur lors du chargement du fichier HTML : ", error);
            });
    }

    setupLogic() {
        /* Vrai constructeur avec EventListener, ...*/
        this.addEventListener("click", () => this.changeVisibility())
    }

    changeVisibility() {
        console.log("call");
        var explications = this.shadowRoot.getElementById("explications");
        if (this.active) {
            explications.classList.remove("display");
            explications.classList.add("no-display");
            this.active = false;
        } else {
            explications.classList.remove("no-display");
            explications.classList.add("display");
            this.active = true;
        }
    }
}

customElements.define("help-component", Help);