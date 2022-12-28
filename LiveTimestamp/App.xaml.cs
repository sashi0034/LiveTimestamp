using LiveTimestamp.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Forms = System.Windows.Forms;

namespace LiveTimestamp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly KeyConfigWindow KeyConfigWindow;
        public readonly TsSettingWindow TsSettingWindow;
        public readonly TimestampMenuWindow TimestampMenuWindow;

        public App()
        {
            InitializeComponent();
            
            KeyConfigWindow = new KeyConfigWindow(onPushedHotKey);
            KeyConfigWindow.ShowInTaskbar = false;

            TimestampMenuWindow = new TimestampMenuWindow(this);
            TimestampMenuWindow.ShowInTaskbar = false;
            TimestampMenuWindow.ShowActivated = false;

            TsSettingWindow = new TsSettingWindow();
            // TODO: フォーマットを可変に
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            checkInitialSetup();

            var icon = GetResourceStream(new Uri("StaticResources/app_icon.ico", UriKind.Relative)).Stream;
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("Configure shortcut key", null, (_, _) => { Util.ShowWindowAboveCursor(KeyConfigWindow); });
            var notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "Live Timestamp",
                ContextMenuStrip = menu
            };
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(onClickIcon);
        }

        private void checkInitialSetup()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                try
                {
                    setupAsAdmin();
                    MessageBox.Show($"Finished setup", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Filed to setup\n" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void onClickIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                Util.ShowWindowAboveCursor(KeyConfigWindow);
            }
        }

        private void onPushedHotKey()
        {
            inputTimestamp();
        }

        private void inputTimestamp()
        {
            if (TimestampMenuWindow.IsActivated) return;
            TimestampMenuWindow.StartPopup();

            this.Dispatcher.Invoke(new Action(async () =>
            {
                await Util.WaitUntil(() => TimestampMenuWindow.IsActivated == false);
                await sendInputTimestamp(TimestampMenuWindow.SelectedFormat);
            }));
        }

        private static async Task sendInputTimestamp(string format)
        {
            var inputContent = DateTime.Now.ToString(format);
            Forms.SendKeys.SendWait(inputContent);
            Debug.WriteLine("sent: " + inputContent);
        }

        private void shutdownApp(object sender, EventArgs e)
        {
            Shutdown();
        }

        /// <summary>
        /// CurrentUserのRunにアプリケーションの実行ファイルパスを登録する
        /// </summary>
        private void setupAsAdmin()
        {
            Microsoft.Win32.RegistryKey registrykey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            registrykey.SetValue(ConstParam.AppProductName, GetCurrentAppDir + @"\" + ConstParam.AppFileName);

            registrykey.Close();
        }

        public static string? GetCurrentAppDir()
        {
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
