using HandSchool.Internals;
using System.Windows.Input;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// 校园卡信息的储存类。
    /// </summary>
    internal class SchoolCardInfoPiece : NotifyPropertyChanged
    {
        string title, description;
        ICommand command;

        /// <summary>
        /// 信息的标题
        /// </summary>
        public virtual string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        /// <summary>
        /// 信息的描述
        /// </summary>
        public virtual string Description
        {
            get => description;
            set => SetProperty(ref description, value, onChanged: () => OnPropertyChanged(nameof(Detail)));
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        public string Detail => Description;

        /// <summary>
        /// 信息的操作
        /// </summary>
        public ICommand Command
        {
            get => command;
            set => SetProperty(ref command, value);
        }

        protected SchoolCardInfoPiece() { }

        public SchoolCardInfoPiece(string tit, string desc, ICommand cmd = null)
        {
            title = tit;
            description = desc;
            command = cmd;
        }
    }
}