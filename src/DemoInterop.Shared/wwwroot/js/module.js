// Mechanisms 2 and 3 — an ES module.
// `calls` is private module state; a window global cannot keep anything between calls.
let calls = 0;

export function getMessage() {
    calls++;
    return `Hello from the ES module — call #${calls}. The counter survived the previous call.`;
}

export function makeRawObject(label) {
    return { label: label, nested: { value: 42 } };
}

// --- freeze measurement for mechanism 3 -------------------------------------
// A synchronous call into slow JavaScript blocks the one thread that paints the page.
// The rAF loop below cannot run while .NET holds the thread, so the gap between two
// painted frames grows to roughly the length of the blocking call.
let watchOn = false;
let worstGapMs = 0;
let previousFrame = 0;

export function startFrameWatch() {
    worstGapMs = 0;
    watchOn = true;
    previousFrame = performance.now();

    const tick = () => {
        if (!watchOn) {
            return;
        }

        const now = performance.now();
        if (now - previousFrame > worstGapMs) {
            worstGapMs = now - previousFrame;
        }
        previousFrame = now;
        requestAnimationFrame(tick);
    };

    requestAnimationFrame(tick);
}

export function stopFrameWatch() {
    watchOn = false;
}

export async function readWorstFrameGap() {
    // The blocking call returns in the same turn it ends, so the long gap is not yet
    // recorded at that moment — it only closes when the next frame is actually painted.
    // Wait for that frame before reading. A timeout keeps this from hanging forever if
    // the tab is hidden, because a hidden tab paints nothing.
    await Promise.race([
        new Promise((resolve) => requestAnimationFrame(resolve)),
        new Promise((resolve) => setTimeout(resolve, 100)),
    ]);

    return Math.round(worstGapMs);
}

export function blockMainThread(milliseconds) {
    const until = Date.now() + milliseconds;
    while (Date.now() < until) {
        // busy-wait on purpose: this is what a synchronous interop call into slow JS costs
    }

    return `blocked for ${milliseconds} ms`;
}
