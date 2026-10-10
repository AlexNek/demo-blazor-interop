// Mechanism 8 — .NET 10 can address a constructor or a property by name,
// so the pass-through shims a wrapper used to need can disappear.
window.vendor = window.vendor || {};

window.vendor.Point = class {
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }

    hypot() {
        return Math.round(Math.hypot(this.x, this.y));
    }
};

window.demoSettings = { name: 'initial', count: 7 };
