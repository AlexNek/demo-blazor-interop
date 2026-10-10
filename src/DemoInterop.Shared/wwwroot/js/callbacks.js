// Mechanism 4 — JavaScript calling back into C# through a DotNetObjectReference.
// Nothing is caught here: the demos show the exception the runtime really produced.

export async function callByMethodName(dotNetRef) {
    await dotNetRef.invokeMethodAsync('OnAnimationEnd');
    return 'the callback ran; C# saw it';
}

// [JSInvokable("Demo.Shared")] replaces the method name, it does not add an alias.
// This is the form the article measured as legal.
export async function callByCustomIdentifier(dotNetRef) {
    await dotNetRef.invokeMethodAsync('Demo.Shared');
    return 'the callback ran through the custom identifier';
}

export async function callByMethodNameAfterCustomIdentifier(dotNetRef) {
    await dotNetRef.invokeMethodAsync('OnAnimationEndWithCustomIdentifier');
    return 'UNEXPECTED: the C# method name dispatched anyway';
}

export async function disposeThenCall(dotNetRef) {
    dotNetRef.dispose();
    await dotNetRef.invokeMethodAsync('OnAnimationEnd');
    return 'UNEXPECTED: the call after dispose succeeded';
}
