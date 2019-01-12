using HandSchool.Internal;
using System;

namespace HandSchool.Views
{
    public class MenuEntry
    {
        public CommandAction Command { get; set; }
        public string Title { get; set; }
        public string UWPIcon { get; set; }
        public Xamarin.Forms.ToolbarItemOrder Order { get; set; }
        public string CommandBinding { get; set; }

        public event EventHandler Execute
        {
            add => Command = new CommandAction(() => value(this, EventArgs.Empty));
            remove => Command = null;
        }
    }
}
