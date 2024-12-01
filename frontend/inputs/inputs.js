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
            let button = shadowRoot.querySelector("button");
            button.addEventListener("click", function() {
                let originComponent = shadowRoot.querySelector('auto-complete-input#start');
                let destinationComponent = shadowRoot.querySelector('auto-complete-input#end');

                if (originComponent && destinationComponent) {
                    let originValue = originComponent.inputValue;
                    let destinationValue = destinationComponent.inputValue;

                    console.log(`Origin: ${originValue}`);
                    console.log(`Destination: ${destinationValue}`);

                    
                    

                    fetch(`http://localhost:8734/Design_Time_Addresses/ServerSide/Service1/path?start=${originValue}&end=${destinationValue}`).then( 
                        response => response.json().then(data => {
                            console.log("DATA: ");
                            console.log(data); // les steps sont de la forme step1|step2|...|stepn où stepi est de la forme (modifier)+type+(exit)+duration+distance+longitude+latitude

                            const pathResult = data.GetPathResult;

                            // Afficher ou manipuler le résultat
                            console.log("GetPathResult: ", pathResult);

                            const steps = pathResult.split('|');

                            console.log("steps: ", steps);

                            // Créer l'élément goal-component
                            const goalComponent = document.createElement("goal-component")
                            goalComponent.addEventListener('attached', () => {
                                console.log("attached");
                                const goal = goalComponent.shadowRoot.querySelector("#goal");

                                if (goal) {
                                    console.log("goal exists");
                                    let distanceSum = 0;
                                    let timeSum = 0;

                                    for (let i=0; i<steps.length; i++) {
                                        console.log("step: "+i);
                                        const splited = steps[i].split('+');
                                        const distance = parseFloat(splited[4]);
                                        const time = parseFloat(splited[3]);
                                        console.log("distance: ", distance);
                                        console.log("time: ", time);

                                        if (distance != undefined && !isNaN(distance)) {
                                            console.log("DISTANCE ADDED");
                                            distanceSum += distance;
                                        }

                                        if (time != undefined && !isNaN(time)) {
                                            console.log("TIME ADDED");
                                            timeSum += time;
                                        }
                                    }
                                    timeSum /= 60;
                                    const totTime = timeSum.toFixed(2);
                                    let minutes = Math.floor(totTime);
                                    let seconds = Math.round((totTime - minutes) * 60);
                                    distanceSum /= 1000;
                                    goal.textContent = distanceSum + "km - " + minutes + ":" + seconds;
                                }
                            })

                            const goalComponentContainer = document.getElementById("goal-component");
                            if (goalComponentContainer) {
                                const existingGoalComponent = goalComponentContainer.querySelector("goal-component");
                                if (existingGoalComponent) {
                                    existingGoalComponent.remove();
                                }

                                goalComponentContainer.appendChild(goalComponent);
                            }

                            const firstStep = steps[1].split('+');
                            const distance = firstStep[4];
                            const modifier = firstStep[1] + ' ' + firstStep[0];

                            // Créer l'élément main_instruction
                            const mainInstruction = document.createElement("main-instruction");
                            mainInstruction.addEventListener('attached', () => {
                                const dataDistance = mainInstruction.shadowRoot.querySelector("#data-distance");
                                const dataText = mainInstruction.shadowRoot.querySelector("#data-text");

                                if (dataDistance && dataText) {
                                    dataDistance.textContent = distance + 'm';
                                    dataText.textContent = modifier;

                                    console.log("distance text : ", dataDistance.textContent);
                                    console.log("data text : ", dataText.textContent);
                                } else {
                                    console.error("data-distance ou data-text introuvables dans le Shadow DOM");
                                }
                            })

                            const mainInstructionContainer = document.getElementById("main_instruction");
                            if (mainInstructionContainer) {
                                const existingMainInstruction = mainInstructionContainer.querySelector("main-instruction");
                                if (existingMainInstruction) {
                                    existingMainInstruction.remove();
                                }

                                mainInstructionContainer.appendChild(mainInstruction);
                            }

                            const event = new CustomEvent('origin-destination-changed', {
                                detail: {
                                    origin: originValue,
                                    destination: destinationValue
                                }
                            });

                            // Émettez l'événement
                            window.dispatchEvent(event);
                        })
                    );
                } else {
                    console.error("Origin or destination component not found!");
                }
            });
        });
    }
}

customElements.define("origin-destination-input", OriginDestinationInput);
