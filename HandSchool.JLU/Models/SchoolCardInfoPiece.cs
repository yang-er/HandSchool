using HandSchool.Internal;

namespace HandSchool.JLU.Models
{
    /// <summary>
    /// 校园卡信息的储存类。
    /// </summary>
    internal class SchoolCardInfoPiece : NotifyPropertyChanged
    {
        string title, description;
        Command command;

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
            set => SetProperty(ref description, value);
        }

        /// <summary>
        /// 信息的操作
        /// </summary>
        public Command Command
        {
            get => command;
            set => SetProperty(ref command, value);
        }

        protected SchoolCardInfoPiece() { }

        public SchoolCardInfoPiece(string tit, string desc, Command cmd = null)
        {
            title = tit;
            description = desc;
            command = cmd;
        }
    }
}
