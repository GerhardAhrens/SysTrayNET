//-----------------------------------------------------------------------
// <copyright file="HotKeyToSendKey.cs" company="Lifeprojects.de">
//     Class: HotKeyToSendKey
//     Copyright © Lifeprojects.de 2025 
// </copyright>
//
// <Framework>4.8</Framework>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>25.12.2022 13:20:07</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace SysTrayNET.Core
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Input;

    using static System.Runtime.CompilerServices.RuntimeHelpers;

    [Serializable]
    public class HotKeyToSendKey : HotKey
    {
        private string name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyToMessageBox"/> class.
        /// </summary>
        public HotKeyToSendKey(string name, Key key, ModifierKeys modifiers, bool enabled = true) : base(key, modifiers, enabled)
        {
            this.Name = name;
            this.PressKey = key;
            this.ModifierKeys = modifiers;
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ModifierKeys ModifierKeys { get; set; }

        private Key PressKey { get; set; }

        protected override void OnHotKeyPress()
        {
            base.OnHotKeyPress();

            Process p = Process.GetProcessesByName("notepad++").FirstOrDefault();
            if (p != null)
            {
                /*
                 * https://github.com/DevInDeep/SendKeys
                 */

                IntPtr h = p.MainWindowHandle;
                if (SetForegroundWindow(h) > 0)
                {
                    try
                    {
                        Clipboard.Clear();
                        Clipboard.SetText("Test");
                        string strClip = Clipboard.GetText();
                        System.Windows.Forms.SendKeys.Send("^{v}");
                    }
                    catch (Exception e)
                    {
                        Clipboard.SetText(e.Message);
                    }
                }
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", this.Name);
        }

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
    }
}
