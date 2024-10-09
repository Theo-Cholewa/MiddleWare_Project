class MainInstruction extends HTMLElement {

    constructor() {
        super();
    }

    connectedCallback() {
        console.log("Call2");
    }
}

customElements.define("main-instruction", MainInstruction);