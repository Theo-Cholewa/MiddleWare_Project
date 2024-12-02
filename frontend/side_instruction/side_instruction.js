class SideInstruction extends HTMLElement {

    constructor() {
        super();
    
        let shadowRoot = this.attachShadow({mode: "open"});

        fetch("/side_instruction/side_instruction.html")
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
        const container = document.querySelector('#side-instructions');

        // Fonction pour afficher uniquement les 4 premiers éléments
        const updateInstructionsDisplay = () => {
            const instructions = container.querySelectorAll('side-instruction');
            instructions.forEach((instruction, index) => {
                if (index >= 4) {
                    instruction.style.display = 'none'; // Masquer les éléments après le 4e
                } else {
                    instruction.style.display = 'block'; // Afficher les 4 premiers
                }
            });
        };

        // Observer pour détecter les ajouts ou suppressions dans le conteneur
        const observer = new MutationObserver(updateInstructionsDisplay);

        // Configurer l'observer pour écouter les changements dans les enfants du conteneur
        observer.observe(container, {
            childList: true, // Observer les ajouts et suppressions d'enfants
            subtree: true // Observer les descendants du conteneur
        });

        // Appeler la fonction au départ pour afficher les 4 premiers éléments
        updateInstructionsDisplay();
    }
}

customElements.define("side-instruction", SideInstruction);