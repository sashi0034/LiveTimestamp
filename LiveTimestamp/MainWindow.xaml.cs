﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiveTimestamp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 登録用ID
        private const int HotKeyId = 0x0001;

        private const int WM_HOTKEY = 0x0312;

        private IntPtr MyWindowHandle;

        // 必要なAPI参照
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);


        public MainWindow()
        {
            InitializeComponent();

            var host = new WindowInteropHelper(this);
            MyWindowHandle = host.Handle;

            //コンボボックス初期化
            comboBoxKey.ItemsSource = Enum.GetValues(typeof(Key));
            comboBoxKey.SelectedIndex = 0;

            ComponentDispatcher.ThreadPreprocessMessage += onPushedHotKey;

            this.Closed += MainWindow_Closed;
        }

        // アプリ終了時に登録解除
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            UnregisterHotKey(MyWindowHandle, HotKeyId);
            ComponentDispatcher.ThreadPreprocessMessage -= onPushedHotKey;
        }



        // ホットキー登録
        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            var (modifier, str) = readModifierChecks();

            var key = (Key)comboBoxKey.SelectedValue;
            UnregisterHotKey(MyWindowHandle, HotKeyId);

            if (RegisterHotKey(MyWindowHandle, HotKeyId, modifier, KeyInterop.VirtualKeyFromKey(key)) == 0)
            {
                MessageBox.Show("登録に失敗");
            }
            else
            {
                str += $" + {key}";
                str = str.Remove(0, 3);
                MessageBox.Show($"{str} を登録しました");
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
            return (modifier, str);
        }


        // ホットキーが押されたときの動作
        private void onPushedHotKey(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY) return;

            switch (msg.wParam.ToInt32())
            {
                case HotKeyId:
                    MessageBox.Show("HotKey1");
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
    }
}
