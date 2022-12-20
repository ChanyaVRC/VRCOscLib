﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;

/// <summary>
/// Represents a delegate that handles OSC parameter change events.
/// </summary>
/// <typeparam name="TSender">The type of the sender of the event.</typeparam>
/// <param name="sender">The sender of the event.</param>
/// <param name="e">The event data.</param>
public delegate void OscParameterChangedEventHandler<TSender>(TSender sender, ParameterChangedEventArgs e);
