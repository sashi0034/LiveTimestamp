using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTimestamp
{
    public static class Util
    {
        public static async Task WaitUntil(Func<bool> func)
        {
            while (true)
            {
                await Task.Delay(ConstParam.DeltaMilliSec);
                if (func() == true) break;
            }
        }
    }
}
