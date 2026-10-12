// Wrapper module — re-exports global functions from legacy.js as ES module exports.
// This is the migration pattern: import the legacy file (which attaches to window),
// then re-export the functions so Blazor can call them through an IJSObjectReference.
import './legacy.js';

export const greet = window.legacyGreet;
export const add = window.legacyAdd;
