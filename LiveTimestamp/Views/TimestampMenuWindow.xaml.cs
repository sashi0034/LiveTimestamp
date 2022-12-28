using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LiveTimestamp.Views
{
    /// <summary>
    /// TimestampMenuWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TimestampMenuWindow : Window
    {
        private bool _isActivated = false;
        public bool IsActivated => _isActivated;

        public TimestampMenuWindow()
        {
            InitializeComponent();
        }

        public void StartPopup()
        {
            _isActivated = true;

            this.Show();
            this.Topmost = true;

            this.Dispatcher.Invoke(async () =>
            {
                await Util.WaitUntil(() => isFinishedConfirm());
                _isActivated = false;
                Hide();
            });
        }

        private static bool isFinishedConfirm()
        {
            return 
                !Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) &&
                !Keyboard.IsKeyDown(Key.LeftAlt) &&
                !Keyboard.IsKeyDown(Key.LWin);
        }
    }
}
