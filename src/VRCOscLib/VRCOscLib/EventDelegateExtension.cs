namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Extension methods for <see cref="Delegate"/>.
/// </summary>
internal static class EventDelegateExtension
{
    /// <summary>
    /// Invokes all of the methods in the specified delegate, catching any exceptions that are thrown.
    /// </summary>
    /// <typeparam name="T">The type of the delegate.</typeparam>
    /// <param name="delegate">The delegate to invoke.</param>
    /// <param name="args">The arguments to pass to the delegate methods.</param>
    public static void DynamicInvokeAllWithoutException<T>(this T @delegate, params object[] args) where T : Delegate
    {
        foreach (var item in @delegate.GetInvocationList())
        {
            try
            {
                item.DynamicInvoke(args);
            }
            catch (Exception)
            {
                // eat exception
            }
        }
    }
}
