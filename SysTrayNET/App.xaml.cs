namespace SysTrayNET
{
    using System.Configuration;
    using System.Data;
    using System.Windows;

    using NotifyIcon = System.Windows.Forms.NotifyIcon;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static NotifyIcon icon;

        protected override void OnStartup(StartupEventArgs e)
        {
            App.icon = new NotifyIcon();
            icon.Click += new EventHandler(icon_Click);
            icon.Icon = new System.Drawing.Icon(typeof(App), "TrayIcon.ico");
            icon.Visible = true;

            base.OnStartup(e);
        }

        private void icon_Click(Object sender, EventArgs e)
        {
            MessageBox.Show("Thanks for clicking me");
        }
    }
}
