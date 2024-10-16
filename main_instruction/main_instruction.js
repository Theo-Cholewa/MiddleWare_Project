class MainInstruction extends HTMLElement {

    constructor() {
        super();
    }

    connectedCallback() {
        // Create a shadow root
        const shadow = this.attachShadow({mode: "open"});

        // Create spans
        const wrapper = document.createElement("div");
        wrapper.setAttribute("class", "wrapper");

        const icon = document.createElement("div");
        icon.setAttribute("class", "icon");
        icon.setAttribute("tabindex", 0);
        
        const info = document.createElement("div");
        info.setAttribute("class", "info");

        const info1 = document.createElement("div");
        info1.setAttribute("class", "info");

        // Insert icon
        let imgUrl = this.getAttribute("img");

        const img = document.createElement("img");
        img.src = imgUrl;
        icon.appendChild(img);

        // Take attribute content and put it inside the info span
        const distance = this.getAttribute("data-distance");
        info.textContent = distance + "m";

        const text = this.getAttribute("data-text");
        info1.textContent = text;

        // Create some CSS
        const style = document.createElement("style");
        console.log(style.isConnected);

        style.textContent = `
            .info {
                background: red;
            }
        `;

        // Attach the created elements to the shadow dom
        shadow.appendChild(style);
        console.log(style.isConnected);
        shadow.appendChild(wrapper);
        wrapper.appendChild(icon);
        wrapper.appendChild(info);
        wrapper.appendChild(info1);
    }
}

customElements.define("main-instruction", MainInstruction);