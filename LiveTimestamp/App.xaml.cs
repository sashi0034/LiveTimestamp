using LiveTimestamp.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
        public readonly ConfigKeyWindow ConfigKeyWindow;
        public readonly TimestampMenuWindow TimestampMenuWindow;

        public App()
        {
            InitializeComponent();
            
            ConfigKeyWindow = new ConfigKeyWindow(onPushedHotKey);
            ConfigKeyWindow.ShowInTaskbar = false;

            TimestampMenuWindow = new TimestampMenuWindow();
            TimestampMenuWindow.ShowInTaskbar = false;
            TimestampMenuWindow.ShowActivated = false;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var icon = GetResourceStream(new Uri("StaticResources/app_icon.ico", UriKind.Relative)).Stream;
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("Configure shortcut key", null, (_, _) => { showConfigKeyWindow(); });
            var notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Visible = true,
                Icon = new System.Drawing.Icon(icon),
                Text = "Live Timestamp",
                ContextMenuStrip = menu
            };
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(onClickIcon);
        }

        private void onClickIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                showConfigKeyWindow();
            }
        }

        private void showConfigKeyWindow()
        {
            var mousePos = System.Windows.Forms.Cursor.Position;
            ConfigKeyWindow.Left = Math.Max(0, mousePos.X - ConfigKeyWindow.Width);
            ConfigKeyWindow.Top = Math.Max(0, mousePos.Y - ConfigKeyWindow.Height);
            ConfigKeyWindow.Show();
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
                await sendInputTimestamp();
            }));
        }

        private static async Task sendInputTimestamp()
        {
            var inputContent = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            Forms.SendKeys.SendWait(inputContent);
            Debug.WriteLine("sent: " + inputContent);
        }

        private void shutdownApp(object sender, EventArgs e)
        {
            Shutdown();
        }
    }
}
