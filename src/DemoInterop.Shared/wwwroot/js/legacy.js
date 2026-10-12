// Legacy JavaScript — global functions attached to window.
// This is the "before" state: code that works with <script> tags
// but cannot be imported as an ES module by Blazor.
(function () {
    window.legacyGreet = function (name) {
        return `Hello, ${name}! (from a global function)`;
    };

    window.legacyAdd = function (a, b) {
        return a + b;
    };
})();
