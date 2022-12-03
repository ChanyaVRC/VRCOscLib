using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;
public delegate void OscValueChangedEventHandler<TSender, T>(TSender sender, ValueChangedEventArgs<T> e);
