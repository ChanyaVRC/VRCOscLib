using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSoft.VRChat.Osc.Test;

public static class TestUtility
{
    public static async Task LoopWhile(Func<bool> conditions, TimeSpan timeout)
    {
        await NewMethod(conditions).WaitAsync(timeout);
        static async Task NewMethod(Func<bool> conditions)
        {
            while (conditions())
            {
                await Task.Delay(10);
            }
        }
    }
}
