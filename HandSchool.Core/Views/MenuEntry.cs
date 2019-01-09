using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.Views
{
    public class MenuEntry
    {
        public Command Command { get; set; }
        public string Title { get; set; }
        public string UWPIcon { get; set; }
        public ToolbarItemOrder Order { get; set; }
        public string CommandBinding { get; set; }

        public event EventHandler Execute
        {
            add => Command = new Command(() => value(this, EventArgs.Empty));
            remove => Command = null;
        }
    }
}
