class MainInstruction extends HTMLElement {

    constructor() {
        super();

        let shadowRoot = this.attachShadow({mode: "open"});
        this.shadowRootInstance = shadowRoot;

        fetch("/main_instruction/main_instruction.html")
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

                this.dispatchEvent(new CustomEvent('attached'));
            })
            .catch(error => {
                console.error("Erreur lors du chargement du fichier HTML : ", error);
            });
    }

    setupLogic() {
        const dataImg = this.shadowRootInstance.querySelector("#data-img");
        const dataDistance = this.shadowRootInstance.querySelector("#data-distance");
        const dataText = this.shadowRootInstance.querySelector("#data-text");

        dataImg.src = "../assets/arrows/turn_right.png";
        dataDistance.textContent = "500m";
        dataText.textContent = "Tournez à droite";
    }
}

customElements.define("main-instruction", MainInstruction);