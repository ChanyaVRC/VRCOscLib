using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;

public delegate void OscAvatarParameterChangedEventHandler(OscAvatarParameter parameter, ValueChangedEventArgs e);
public delegate void OscValueChangedEventHandler<TSender, T>(TSender sender, ValueChangedEventArgs<T> e);
