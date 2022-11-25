namespace BuildSoft.VRChat.Osc.Delegate;

internal static class EventDelegateExtension
{
    public static void DynamicInvokeAllWithoutException<T>(this T @delegate, params object[] args) where T : System.Delegate
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
