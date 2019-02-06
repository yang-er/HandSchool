using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandSchool.Internals
{
    /// <summary>
    /// 命令，可以设置触发按钮后的操作。
    /// </summary>
    public class CommandAction : ICommand
    {
        readonly Action<object> action;

        public CommandAction(Action<object> command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = command;
        }

        public CommandAction(Action command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = (o) => command();
        }

        public CommandAction(Func<Task> command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = async (o) => await command();
        }

        public CommandAction(Func<object, Task> command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = async (o) => await command(o);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

#pragma warning disable CS0067

        // readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

#pragma warning restore
    }
}
