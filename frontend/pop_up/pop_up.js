class PopUp extends HTMLElement {

    constructor() {
        super();
    
        let shadowRoot = this.attachShadow({mode: "open"});
        this.shadowRootInstance = shadowRoot;

        fetch("/pop_up/pop_up.html")
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
        const dataImg = this.shadowRootInstance.querySelector("#data-img");
        const dataText = this.shadowRootInstance.querySelector("#data-text");

        dataImg.src = "../assets/arrows/turn_right.png";
        dataText.textContent = "1km - Accident";
    }
}

customElements.define("pop-up", PopUp);