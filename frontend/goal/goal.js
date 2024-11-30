class Goal extends HTMLElement {

    constructor() {
        super();
    
        let shadowRoot = this.attachShadow({mode: "open"});
        this.shadowRootInstance = shadowRoot;

        fetch("/goal/goal.html")
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
        const data = this.shadowRootInstance.querySelector("#goal");

        data.textContent = "10km - 15 min";
    }
}

customElements.define("goal-component", Goal);