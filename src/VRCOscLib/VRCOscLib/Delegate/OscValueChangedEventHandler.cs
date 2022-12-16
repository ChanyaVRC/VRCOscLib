using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Represents a delegate that handles value change events.
/// </summary>
/// <typeparam name="TSender">The type of the sender of the event.</typeparam>
/// <typeparam name="T">The type of the value that was changed.</typeparam>
/// <param name="sender">The sender of the event.</param>
/// <param name="e">The event data.</param>
public delegate void OscValueChangedEventHandler<TSender, T>(TSender sender, ValueChangedEventArgs<T> e);
