class PopUp extends HTMLElement {

    constructor() {
        super();
    
        let shadowRoot = this.attachShadow({mode: "open"});

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
    }
    // type = accident 
    // message = "accident à 100m"
    addPopUp(type, message) {
        let context = this;
        
        let list = this.shadowRoot.querySelector("div");
  
        let p = document.createElement("p");
        p.innerHTML = message;
        p.classList.add('pop-up');
        list.appendChild(p);
        console.log("pop-up added");
    }
}

customElements.define("pop-up", PopUp);