namespace SysTrayNET.Behaviors
{
    using Microsoft.Xaml.Behaviors;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;

    public class NotifyTrayIconBehavior : Behavior<Window>, IDisposable
    {
        private NotifyIcon notifyTrayIcon;
        private bool disposedValue;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.Closing += AssociatedObject_Closing;
            this.AssociatedObject.StateChanged += AssociatedObject_StateChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            this.AssociatedObject.Closing -= AssociatedObject_Closing;
            this.AssociatedObject.StateChanged -= AssociatedObject_StateChanged;
            this.Dispose();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeNotifyIcon();
        }

        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.AssociatedObject.Hide();

            this.ShowNotificationInTray(string.Empty, "Dieser Hinweis zeigt an, dass Sie über das Symbol in der Taskleiste auf Ihre Anwendung zugreifen können.");
        }

        private void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            if (this.AssociatedObject.WindowState == WindowState.Minimized)
            {
                this.AssociatedObject.ShowInTaskbar = false;
                this.ShowNotificationInTray(string.Empty, "Dieser Hinweis zeigt an, dass Sie über das Symbol in der Taskleiste auf Ihre Anwendung zugreifen können.");
            }
            else
            {
                this.AssociatedObject.ShowInTaskbar = true;
            }
        }

        private void InitializeNotifyIcon()
        {
            this.notifyTrayIcon = new NotifyIcon
            {
                Icon = GetIconFromImageSource(new Uri("pack://application:,,,/SysTrayNET;component/Resources/Picture/TrayIcon.ico")),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            ContextMenuStrip contextMenu = new();
            ToolStripMenuItem exitNotifyIconMenuItem = new()
            {
                Text = "Beenden"
            };

            ToolStripMenuItem openNotifyIconMenuItem = new()
            {
                Text = "Dialog wiederherstellen",
            };

            WeakEventManager<ToolStripMenuItem, EventArgs>.AddHandler(openNotifyIconMenuItem, "Click", this.OpenMenuItem_Click);
            WeakEventManager<ToolStripMenuItem, EventArgs>.AddHandler(exitNotifyIconMenuItem, "Click", this.ExitMenuItem_Click);

            contextMenu.Items.Add(openNotifyIconMenuItem);
            contextMenu.Items.Add(exitNotifyIconMenuItem);
            this.notifyTrayIcon.ContextMenuStrip = contextMenu;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyTrayIcon?.Dispose();
            Environment.Exit(0);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            this.AssociatedObject.Show();
            this.AssociatedObject.WindowState = WindowState.Normal;
            this.AssociatedObject.Activate();
        }

        private void ShowNotificationInTray(string title, string message)
        {
            this.notifyTrayIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Info);
            WeakEventManager<NotifyIcon, EventArgs>.AddHandler(this.notifyTrayIcon, "DoubleClick", this.NotifyTrayIcon_DoubleClick);
        }

        private void NotifyTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.AssociatedObject.Show();
            this.AssociatedObject.WindowState = WindowState.Normal;
            this.AssociatedObject.Activate();
        }

        private static Icon GetIconFromImageSource(Uri uri)
        {
            using var stream = System.Windows.Application.GetResourceStream(uri)?.Stream;
            return stream != null ? new Icon(stream) : null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue == false)
            {
                if (disposing == true)
                {
                    this.notifyTrayIcon?.Dispose();
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
