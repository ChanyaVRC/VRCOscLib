using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;
public class ValueChangedEventArgs<T> : EventArgs
{
    public T OldValue;
    public T NewValue;

    public ValueChangedEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
