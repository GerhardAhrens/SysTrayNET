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
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Automation;

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

            string textToInsert = "Hallo von UI Automation!";
            Clipboard.SetText(textToInsert);

            var processes = Process.GetProcessesByName("winword").FirstOrDefault();
            if (processes == null)
            {
                return;
            }

            _ = SetForegroundWindow(processes.MainWindowHandle);

            Thread.Sleep(100);
            //Strg+V über SendInput simulieren
            SendCtrlV();
            // Optional: kurze Pause zwischen Fenstern
            Thread.Sleep(200);

            /*
            var root = AutomationElement.FromHandle(processes.MainWindowHandle);
            var edit = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document));
            if (edit != null)
            {
                var pattern = edit.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (pattern != null)
                {
                    _ = SetForegroundWindow(processes.MainWindowHandle);

                    Thread.Sleep(100);
                    //Strg+V über SendInput simulieren
                    SendCtrlV();
                    // Optional: kurze Pause zwischen Fenstern
                    Thread.Sleep(200);

                    //var vp = (ValuePattern)pattern;
                    //vp.SetValue(textToInsert); // Text wird direkt gesetzt
                }

            }
            */
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", this.Name);
        }

        static void SendCtrlV()
        {
            // Ctrl down
            SendKey(VK_CONTROL, false);
            // V down
            SendKey(VK_V, false);
            // V up
            SendKey(VK_V, true);
            // Ctrl up
            SendKey(VK_CONTROL, true);
        }

        static void SendKey(ushort vk, bool keyUp)
        {
            INPUT[] inputs = new INPUT[]
            {
            new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = vk,
                        wScan = 0,
                        dwFlags = keyUp ? KEYEVENTF_KEYUP : 0,
                        time = 0,
                        dwExtraInfo = IntPtr.Zero
                    }
                }
            }
            };

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // --- Win32 API ---
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)] public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        const int INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const ushort VK_CONTROL = 0x11;
        const ushort VK_V = 0x56;
    }
}
