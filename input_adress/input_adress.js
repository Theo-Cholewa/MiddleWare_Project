class InputAdress extends HTMLElement {

    constructor() {
        super();

        let shadowRoot = this.attachShadow({mode: "open"});

        fetch("/input_adress/input_adress.html")
            .then(response => {
                if (!response.ok) {
                    throw new Error('Échec du chargement du fichier HTML');
                }
                return response.text();
            })
            .then(htmlContent => {
                let templateContent = new DOMParser().parseFromString(htmlContent, "text/html").querySelector("template").content;
                console.log("1"); 
                shadowRoot.appendChild(templateContent.cloneNode(true));
                this.setupLogic(); // Appeler la fonction pour configurer la logique du composant
                console.log("2"); 
            })
            .catch(error => {
                console.error("Erreur lors du chargement du fichier HTML : ", error);
            });
    }

    setupLogic() {
        /* Vrai constructeur avec EventListener, ...*/
        //alert("Le composant input adress est maintenant visible sur la page !");
        console.log("sdfsd"); 
        //console.log("input adress chargé");   
    }
}

customElements.define("input-adress", InputAdress);