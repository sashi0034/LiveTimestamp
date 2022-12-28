using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveTimestamp.Utils
{
    class KeyDownChecker
    {
        private bool isPushed = false;
        private readonly Action onKeyDown;
        private CancellationTokenSource _cancellationCheck = new CancellationTokenSource();

        public KeyDownChecker(Action onKeyDown)
        {
            this.onKeyDown = onKeyDown;
        }

        public void StartCheck(Key checkingKey)
        {
            _cancellationCheck.Cancel();
            _cancellationCheck = new CancellationTokenSource();
            isPushed = Keyboard.IsKeyDown(checkingKey);
            checkAsync(checkingKey, _cancellationCheck.Token);
        }
        public void StopCheck() 
        { 
            _cancellationCheck.Cancel();
        }
        private async Task checkAsync(Key checkingKey, CancellationToken cancel)
        {
            while (true)
            {
                if (cancel.IsCancellationRequested) return;

                if (Keyboard.IsKeyDown(checkingKey))
                {
                    if (isPushed == false) onKeyDown();
                    isPushed = true;
                }
                else
                {
                    isPushed = false;
                }

                await Task.Delay(ConstParam.DeltaMilliSec, cancel);
            }
        }
    }
}
