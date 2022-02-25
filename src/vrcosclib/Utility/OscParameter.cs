using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSoft.VRChat.Osc;
public static class OscParameter
{
    private static OscParameterCollection? _parameters;
    internal static OscParameterCollection Parameters => _parameters ??= new();

    static OscParameter()
    {
        OscParameterReceiver.Initialize();
    }

}
