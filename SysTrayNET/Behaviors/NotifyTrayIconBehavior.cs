namespace SysTrayNET.Behaviors
{
    using Microsoft.Xaml.Behaviors;

    using System.Windows;

    public class NotifyTrayIconBehavior : Behavior<Window>
    {
        private NotifyIcon notifyTrayIcon;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Closing += AssociatedObject_Closing;
            AssociatedObject.StateChanged += AssociatedObject_StateChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Closing -= AssociatedObject_Closing;
            AssociatedObject.StateChanged -= AssociatedObject_StateChanged;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeNotifyIcon();
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            AssociatedObject.Hide();
            ShowNotificationInTray(string.Empty, "Dieser Hinweis zeigt an, dass Sie über das Symbol in der Taskleiste auf Ihre Anwendung zugreifen können.");
        }

        private void AssociatedObject_StateChanged(object sender, EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Minimized)
            {
                AssociatedObject.ShowInTaskbar = false;
                ShowNotificationInTray(string.Empty, "Dieser Hinweis zeigt an, dass Sie über das Symbol in der Taskleiste auf Ihre Anwendung zugreifen können.");
            }
            else
            {
                AssociatedObject.ShowInTaskbar = true;
            }
        }

        private void InitializeNotifyIcon()
        {
            // Initialize NotifyIcon
            this.notifyTrayIcon = new NotifyIcon
            {
                /* Geben Sie das Symbol an, das im Infobereich angezeigt werden soll. */
                Icon = GetIconFromImageSource(new Uri("pack://application:,,,/SysTrayNET;component/Resources/Picture/TrayIcon.ico")),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            /* Fügen Sie ein Kontextmenü zum NotifyIcon hinzu. */
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
            // Perform cleanup and exit the application
            this.notifyTrayIcon?.Dispose();
            Environment.Exit(0);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            AssociatedObject.Show();
            AssociatedObject.WindowState = WindowState.Normal;
            AssociatedObject.Activate();
        }

        private void ShowNotificationInTray(string title, string message)
        {
            /* Um einen Balloon-Tip anzuzeigen, verwenden Sie die Funktion notifyTrayIcon und geben Sie die 
             * Dauer (2000 Millisekunden), den Titel, die Meldung und den Symboltyp (ToolTipIcon.Info) an.*/
            this.notifyTrayIcon.ShowBalloonTip(2000, title, message, ToolTipIcon.Info);
            WeakEventManager<NotifyIcon, EventArgs>.AddHandler(notifyTrayIcon, "DoubleClick", this.NotifyTrayIcon_DoubleClick);
        }

        private void NotifyTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            AssociatedObject.Show();
            AssociatedObject.WindowState = WindowState.Normal;
            AssociatedObject.Activate();
        }

        private static Icon GetIconFromImageSource(Uri uri)
        {
            using var stream = System.Windows.Application.GetResourceStream(uri)?.Stream;
            return stream != null ? new Icon(stream) : null;
        }
    }
}
