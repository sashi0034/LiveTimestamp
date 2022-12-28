using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LiveTimestamp.Views
{
    /// <summary>
    /// WindowConfigKey.xaml の相互作用ロジック
    /// 参考: https://gogowaten.hatenablog.com/entry/2020/12/11/132125
    /// </summary>
    public partial class ConfigKeyWindow : Window
    {
        // 登録用ID
        private const int hotKeyId = 0x0001;

        private const int wmHotKey = 0x0312;

        private readonly IntPtr windowHandle;

        private readonly Action onPushedHotKey;

        // 必要なAPI参照
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public Key GetSelectedKey()
        {
            return (Key)(comboBoxKey.SelectedValue);
        }


        public ConfigKeyWindow(Action onPushedHotKey)
        {
            InitializeComponent();

            var host = new WindowInteropHelper(this);
            windowHandle = host.Handle;

            // コンボボックス初期化
            comboBoxKey.ItemsSource = Enum.GetValues(typeof(Key));
            comboBoxKey.SelectedIndex = 0;

            ComponentDispatcher.ThreadPreprocessMessage += onReceivedMessage;
            this.onPushedHotKey = onPushedHotKey;

            setupConfig();
        }

        private void setupConfig()
        {
            checkCtrl.IsChecked = true;
            checkAlt.IsChecked = true;
            checkShift.IsChecked = true;
            comboBoxKey.SelectedValue = Key.T;

            registerKey();
        }

        private (int, string) registerKey()
        {
            var (modifier, keyBind) = readModifierChecks();
            var key = GetSelectedKey();
            keyBind += keyBind != "" ? $" + {key}" : $"{key}";
            int result = RegisterHotKey(windowHandle, hotKeyId, modifier, KeyInterop.VirtualKeyFromKey(key));
            return (result, keyBind);
        }

        // アプリ終了時に登録解除
        private void window_Closed(object sender, EventArgs e)
        {
            UnregisterHotKey(windowHandle, hotKeyId);
            ComponentDispatcher.ThreadPreprocessMessage -= onReceivedMessage;
        }



        // ホットキー登録
        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            UnregisterHotKey(windowHandle, hotKeyId);
            var (result, keyBind) = registerKey();

            if (result == 0)
            {
                MessageBox.Show("登録に失敗");
            }
            else
            {
                MessageBox.Show($"{keyBind} を登録しました");
            }
        }

        private (int, string) readModifierChecks()
        {
            int modifier = 0;
            string str = "";
            if (checkAlt.IsChecked == true) { str += $" + Alt"; modifier = (int)ModifierKeys.Alt; }
            if (checkCtrl.IsChecked == true) { str += $" + Ctrl"; modifier += (int)ModifierKeys.Control; }
            if (checkShift.IsChecked == true) { str += $" + Shift"; modifier += (int)ModifierKeys.Shift; }
            if (checkWin.IsChecked == true) { str += $" + Win"; modifier += (int)ModifierKeys.Windows; }
            return (modifier, str!="" ? str.Remove(0, 3) : "");
        }


        // ホットキーが押されたときの動作
        private void onReceivedMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != wmHotKey) return;

            switch (msg.wParam.ToInt32())
            {
                case hotKeyId:
                    onPushedHotKey();
                    break;
                default:
                    break;
            }
        }


        // コンボボックス上でキーを押し下げたとき
        // 入力されたキー文字は無視
        private void comboBoxKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true; // キーイベント無視
        }

        // コンボボックス上でキーが上げられたとき
        // 修飾キー以外なら、そのキーと同じキーをコンボボックスで選択する
        // 文字は無視
        private void comboBoxKey_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if ((key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin) == false)
            {
                comboBoxKey.SelectedValue = key;
            }

            e.Handled = true;
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
