//-----------------------------------------------------------------------
// <copyright file="HotKeyToApplicationOpen.cs" company="Lifeprojects.de">
//     Class: HotKeyToApplicationOpen
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
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Input;

    [Serializable]
    public class HotKeyToApplicationOpen : HotKey
    {
        private string name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyToMessageBox"/> class.
        /// </summary>
        public HotKeyToApplicationOpen(string name, Key key, ModifierKeys modifiers, Window window, bool enabled = true) : base(key, modifiers, enabled)
        {
            this.Name = name;
            this.PressKey = key;
            this.ModifierKeys = modifiers;
            this.Window = window;
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

        private Window Window { get; set; }

        protected override void OnHotKeyPress()
        {
            base.OnHotKeyPress();

            this.Window.Show();
            this.Window.WindowState = WindowState.Normal;
            this.Window.Activate();

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", this.Name);
        }
    }
}
