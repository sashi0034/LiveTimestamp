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

            TimestampMenuWindow = new TimestampMenuWindow(this);
            TimestampMenuWindow.ShowInTaskbar = false;
            TimestampMenuWindow.ShowActivated = false;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var icon = GetResourceStream(new Uri("StaticResources/app_icon.ico", UriKind.Relative)).Stream;
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add("Configure shortcut key", null, (_, _) => { Util.ShowWindowAboveCursor(ConfigKeyWindow); });
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
                Util.ShowWindowAboveCursor(ConfigKeyWindow);
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
    }
}
