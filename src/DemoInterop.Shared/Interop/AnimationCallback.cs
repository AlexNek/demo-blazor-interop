using Microsoft.JSInterop;

namespace DemoInterop.Shared.Interop;

/// <summary>
/// Mechanism 4: the C# object a JavaScript callback runs on.
/// </summary>
public class AnimationCallback
{
    public int HitCount { get; private set; }

    /// <summary>Registered under its own name, so JavaScript calls invokeMethodAsync("OnAnimationEnd").</summary>
    [JSInvokable]
    public void OnAnimationEnd() => HitCount++;

    /// <summary>
    /// A custom identifier replaces the method name, it does not add an alias.
    /// JavaScript must use "Demo.Shared"; calling by the C# method name fails.
    /// </summary>
    [JSInvokable("Demo.Shared")]
    public void OnAnimationEndWithCustomIdentifier() => HitCount++;
}
