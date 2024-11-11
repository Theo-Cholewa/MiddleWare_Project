class MainInstruction extends HTMLElement {

    constructor() {
        super();

        let shadowRoot = this.attachShadow({mode: "open"});

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
            })
            .catch(error => {
                console.error("Erreur lors du chargement du fichier HTML : ", error);
            });
    }

    setupLogic() {
        /* Vrai constructeur avec EventListener, ...*/
        alert("Le composant est maintenant visible sur la page !");
    }
}

customElements.define("main-instruction", MainInstruction);