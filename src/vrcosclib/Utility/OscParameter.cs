using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;
public static partial class OscParameter
{
    private static OscParameterCollection? _parameters;
    internal static OscParameterCollection Parameters => _parameters ??= new();

    static OscParameter()
    {
        OscUtility.Server.AddMonitorCallback(ReceiveMessage);
    }

    internal static void Initialize()
    {

    }
}
