// Mechanism 1 — window globals.
// Fine for a one-off call inside an application. A published library must not ship this:
// the global name can collide with another package.
window.demoGreeter = function (name) {
    return `Hello, ${name} — this string came out of a window global.`;
};
