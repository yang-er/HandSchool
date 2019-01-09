using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandSchool.Internal
{
    /// <summary>
    /// 命令，可以设置触发按钮后的操作。
    /// </summary>
    public class Command : ICommand
    {
        readonly Action<object> action;

        public Command(Action<object> command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = command;
        }

        public Command(Action command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = (o) => command();
        }

        public Command(Func<Task> command)
        {
            if (command is null)
                throw new ArgumentNullException();
            action = async (o) => await command();
        }

        public Command(Func<object, Task> command)
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
