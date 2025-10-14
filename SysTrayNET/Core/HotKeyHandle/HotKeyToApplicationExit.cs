//-----------------------------------------------------------------------
// <copyright file="CustomHotKey.cs" company="Lifeprojects.de">
//     Class: CustomHotKey
//     Copyright © Lifeprojects.de 2022
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
    using System.Windows.Input;

    [Serializable]
    public class HotKeyToApplicationExit : HotKey
    {
        private string name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyToMessageBox"/> class.
        /// </summary>
        public HotKeyToApplicationExit(string name, Key key, ModifierKeys modifiers, bool enabled = true) : base(key, modifiers, enabled)
        {
            this.Name = name;
            this.PressKey = key;
            this.ModifierKeys = modifiers;
        }

        protected HotKeyToApplicationExit(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Name = info.GetString("Name");
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
            Environment.Exit(0);
            base.OnHotKeyPress();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", this.Name);
        }
    }
}
