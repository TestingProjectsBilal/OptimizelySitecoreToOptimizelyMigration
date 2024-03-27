import { html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { BaseElement } from "./BaseElement";

@customElement("my-counter")
export class Counter extends BaseElement {
    constructor() {
        super();
        this.count = 0;
    }

    @property({ type: Number })
    initialCount?: number;

    @property({ type: Number })
    step = 1;

    @property({ type: Number })
    maxValue?: number;

    @property({ type: Number })
    minValue?: number; // Minimum limit for the count

    @property({ type: String })
    customMessage = "Click me"; // Custom button text

    @property({ type: Number })
    count = 0;

    firstUpdated() {
        if (typeof this.initialCount === 'number') {
            this.count = this.initialCount;
        }
    }

    increment() {
        if (typeof this.maxValue !== 'number' || this.count + this.step <= this.maxValue) {
            this.count += this.step;
        }
    }

    decrement() {
        if (typeof this.minValue !== 'number' || this.count - this.step >= this.minValue) {
            this.count -= this.step;
        }
    }

    render() {
        return html`
            <h2>Counter</h2>
            <p role="status">Current count: ${this.count}</p>
            <button class="btn btn-primary" @click="${this.increment}">${this.customMessage}</button>
            <button class="btn btn-secondary" @click="${this.decrement}">Decrement</button>
        `;
    }
}





