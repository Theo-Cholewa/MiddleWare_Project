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
            document.addEventListener("keydown", function(event) {
                if (event.key !== "Enter") {
                    return;
                }
                let originComponent = shadowRoot.querySelector('auto-complete-input#start');
                let destinationComponent = shadowRoot.querySelector('auto-complete-input#end');

                if (originComponent && destinationComponent) {
                    let originValue = originComponent.inputValue;
                    let destinationValue = destinationComponent.inputValue;

                    console.log(`Origin: ${originValue}`);
                    console.log(`Destination: ${destinationValue}`);                   

                    fetch(`http://localhost:8734/RoutingServer/path?start=${originValue}&end=${destinationValue}`).then( 
                        response => response.json().then(data => {
                            if(data.GetPathResult === "Aucun chemin trouvé."){
                                console.log("Aucun chemin trouvé.");
                                return;
                            }
                            const pathResult = data.GetPathResult;

                            // Récupérer les latitudes et longitudes de chaque step
                            // les steps sont de la forme step1|step2|...|stepn où stepi est de la forme (modifier)+type+(exit)+duration+distance+longitude+latitude
                            console.log("GetPathResult: ", pathResult);

                            // Récupérer le tableau de steps
                            const steps = pathResult.split('|');
                            console.log("steps: ", steps);

                            // Récupérer les coordonnées de chaque step
                            const coords = ParseSteps(steps);

                            // Récupérer les index de changement de couleur
                            colours = GetColours(steps)

                            // Envoyer l'event
                            console.log("send event");
                            const sendPath = new CustomEvent('pathUpdated', {
                                detail: {
                                    coord: coords,
                                    col: colours
                                }
                            });

                            window.dispatchEvent(sendPath);

                            // Supprimer toutes les anciennes side-instructions
                            RemoveSideinstruction();

                            // Créer les side-instructions
                            GenerateSideInstructions(steps);

                            // Créer l'élément goal-component
                            GenerateGoalComponent(steps);

                            // Créer l'élément main-instruction
                            GenerateMainInstruction(steps);
                        })
                    );
                } else {
                    console.error("Origin or destination component not found!");
                }
            });
        });
    }
}

/**
 * Récupère le conteneur des side-instructions et le vide
 */
function RemoveSideinstruction() {
    const sideInstructionContainer = document.getElementById("side-instructions");

    if (sideInstructionContainer) {
        const existingSideInstructions = sideInstructionContainer.querySelector("side-instruction");
        if (existingSideInstructions) {
            for (let j=0; j<existingSideInstructions.length; j++) {
                existingSideInstructions[j].remove();
            }
        }
    }
}

/**
 * Génère sur la page les 4 premières side-instructions
 * 
 * @param steps : L'ensemble des étapes du trajet 
 */
function GenerateSideInstructions(steps) {
    const sideInstructionContainer = document.getElementById("side-instructions");

    for (let i=2; i<steps.length-1; i++) {
        const sideInstruction = document.createElement("side-instruction");
        sideInstruction.addEventListener('attached', () => {
            const data = steps[i].split('+');
            const dataDistance = data[4];
            const modifier = data[1];
            let img_data = "";
            if (modifier == "end of road") {
                img_data += "end_of_road" + '_';
            } else if (modifier == "arrive" && i != steps.length-2) {
                img_data += "velo" + '_'
            } else {
                img_data += data[1] + '_';
            }
            if (modifier != "depart" && modifier != "arrive" && modifier != "end of road") {
                img_data += data[0];
            }
            img_data += ".png";

            const img = sideInstruction.shadowRoot.querySelector("img");
            const distance = sideInstruction.shadowRoot.querySelector("#data-distance");

            if (distance && img) {
                img.src = "../assets/arrows/" + img_data;
                distance.textContent = dataDistance + "m";

                sideInstructionContainer.appendChild(sideInstruction)
            } else {
                console.error("data-distance introuvable dans le Shadow DOM");
            }
        })
    }
}

/**
 * Génère le composant goal (km - temps)
 * 
 * @param steps : L'ensemble des étapes du trajet 
 */
function GenerateGoalComponent(steps) {
    const goalComponent = document.createElement("goal-component")
    goalComponent.addEventListener('attached', () => {
        const goal = goalComponent.shadowRoot.querySelector("#goal");

        if (goal) {
            let distanceSum = 0;
            let timeSum = 0;

            for (let i=0; i<steps.length; i++) {
                const splited = steps[i].split('+');
                const distance = parseFloat(splited[4]);
                const time = parseFloat(splited[3]);

                if (distance != undefined && !isNaN(distance)) {
                    distanceSum += distance;
                }

                if (time != undefined && !isNaN(time)) {
                    timeSum += time;
                }
            }
            console.log("distance: ", distanceSum);
            console.log("time: ", timeSum);
            timeSum /= 60;
            const totTime = timeSum.toFixed(2);
            let minutes = Math.floor(totTime);
            let seconds = Math.round((totTime - minutes) * 60);
            distanceSum /= 1000;
            goal.textContent = distanceSum + "km - " + minutes + ":" + seconds;
        }
    })

    // Remplacer l'ancien goal-component s'il existe
    ReplaceGoalComponent(goalComponent);
}

/**
 * Supprime l'ancien goal-component s'il existe et le remplace par celui en paramètre
 * 
 * @param goalComponent : Nouveau goal-component 
 */
function ReplaceGoalComponent(goalComponent) {
    const goalComponentContainer = document.getElementById("goal-component");
    if (goalComponentContainer) {
        const existingGoalComponent = goalComponentContainer.querySelector("goal-component");
        if (existingGoalComponent) {
            existingGoalComponent.remove();
        }

        goalComponentContainer.appendChild(goalComponent);
    }
}

/**
 * Génère la première instruction du parcours
 * 
 * @param steps : L'ensemble des étapes du trajet 
 */
function GenerateMainInstruction(steps) {
    const firstStep = steps[1].split('+');
    const distance = firstStep[4];
    const modifier = firstStep[1] + ' ' + firstStep[0];

    // Créer l'élément main_instruction
    const mainInstruction = document.createElement("main-instruction");
    mainInstruction.addEventListener('attached', () => {
        let img_data = "";
        if (modifier == "end of road") {
            img_data += "end_of_road" + '_';
        } else if (modifier == "arrive" && i != steps.length-2) {
            img_data += "velo" + '_'
        } else {
            img_data += firstStep[1] + '_';
        }
        if (modifier != "depart" && modifier != "arrive" && modifier != "end of road") {
            img_data += firstStep[0];
        }
        img_data += ".png";

        const dataImg = mainInstruction.shadowRoot.querySelector("#data-img");
        const dataDistance = mainInstruction.shadowRoot.querySelector("#data-distance");
        const dataText = mainInstruction.shadowRoot.querySelector("#data-text");

        if (dataImg && dataDistance && dataText) {
            dataImg.src = "../assets/arrows/" + img_data;
            dataDistance.textContent = distance + 'm';
            dataText.textContent = modifier;

            console.log("distance text : ", dataDistance.textContent);
            console.log("data text : ", dataText.textContent);
        } else {
            console.error("data-distance ou data-text introuvables dans le Shadow DOM");
        }
    })

    // Remplacer l'ancienne main-instruction si elle existe
    ReplaceMainInstruction(mainInstruction);
}

/**
 * Supprime l'ancienne main-instruction si elle existe et la remplace par celle en paramètre
 * 
 * @param mainInstruction :  Nouvelle main-instruction
 */
function ReplaceMainInstruction(mainInstruction) {
    const mainInstructionContainer = document.getElementById("main_instruction");
    if (mainInstructionContainer) {
        const existingMainInstruction = mainInstructionContainer.querySelector("main-instruction");
        if (existingMainInstruction) {
            existingMainInstruction.remove();
        }

        mainInstructionContainer.appendChild(mainInstruction);
    }
}

/**
 * Parse les étapes pour récupérer les coordonnées de chacunes
 * 
 * @param steps : L'ensemble des étapes du trajet 
 * @returns un tableau avec les coordonnées de chaque étape
 */
function ParseSteps(steps) {
    let coords = [];

    for (let i=0; i<steps.length-1; i++) {
        const currentStep = steps[i].split('+');

        const currentStepLng = currentStep[5].replace(',', '.');
        const currentStepLat = currentStep[6].replace(',', '.');

        const tab = [currentStepLat, currentStepLng];
        coords.push(tab);
    }

    return coords;
}

/**
 * Créer un tableau de couleurs pour la polyline
 * 
 * @param steps : L'ensemble des étapes du trajet 
 * @returns un tableau de string rempli de couleur
 */
function GetColours(steps) {
    colours = [0];
    for (let i=1; i<steps.length; i++) {
        if (steps[i].split('+')[1] == "depart") {
            colours.push(i);
        }
    }
    console.log("colours: ", colours);
    return colours;
}

customElements.define("origin-destination-input", OriginDestinationInput);
