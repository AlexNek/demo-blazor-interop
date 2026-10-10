// Mechanism 6 — the module a [JSImport] attribute binds to.
// Registered with JSHost.ImportAsync("demo-native", ...); the two names must agree.

export function getMessage() {
    return 'Hello from a module loaded by JSHost.ImportAsync';
}
