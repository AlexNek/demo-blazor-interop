// Mechanism 7 — stream references: binary payloads instead of Base64 JSON.

export function bigBytes() {
    return new Uint8Array(1_000_000);
}

export async function receiveStream(streamRef) {
    const buffer = await streamRef.arrayBuffer();
    return `JavaScript received ${buffer.byteLength} bytes from .NET`;
}
