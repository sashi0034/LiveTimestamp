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

        public static void ShowWindowAboveCursor(System.Windows.Window window)
        {
            var mousePos = System.Windows.Forms.Cursor.Position;
            window.Left = Math.Max(0, mousePos.X - window.Width);
            window.Top = Math.Max(0, mousePos.Y - window.Height);
            window.Show();
        }
    }
}
