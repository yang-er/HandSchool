using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace HandSchool.JLU.Models
{
    internal sealed class CardBasicInfo : List<SchoolCardInfoPiece>, INotifyPropertyChanged
    {
        public SchoolCardInfoPiece NameInfo { get; set; }
        public SchoolCardInfoPiece CardId { get; set; }
        public SchoolCardInfoPiece Balance { get; set; }
        public SchoolCardInfoPiece InternalUID { get; set; }
        public SchoolCardInfoPiece CurGd { get; set; }
        public SchoolCardInfoPiece Lost { get; set; }
        public SchoolCardInfoPiece Frozen { get; set; }

        public string IsLost { get; set; } = "? 未知";
        public string IsFrozen { get; set; } = "? 未知";

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == Frozen)
            {
                IsFrozen = Frozen.Description.Contains("正常") ? "√ 未冻结" : "× 已冻结";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFrozen)));
            }
            else if (sender == Lost)
            {
                IsLost = Lost.Description.Contains("正常") ? "√ 未挂失" : "× 已挂失";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLost)));
            }
        }

        public CardBasicInfo()
        {
            NameInfo = new SchoolCardInfoPiece("姓名", "未知");
            CardId = new SchoolCardInfoPiece("学工号", "00000000000");
            Balance = new SchoolCardInfoPiece("校园卡余额", "不清楚");
            CurGd = new SchoolCardInfoPiece("当前过渡余额", "听不清");
            Lost = new SchoolCardInfoPiece("挂失状态", "可能正常吧");
            Frozen = new SchoolCardInfoPiece("冻结状态", "大概正常吧");
            InternalUID = new SchoolCardInfoPiece("内部账号", "布吉岛");

            Frozen.PropertyChanged += OnPropertyChanged;
            Lost.PropertyChanged += OnPropertyChanged;

            Add(NameInfo); Add(CardId); Add(Balance);
            Add(CurGd);
            Add(Lost); Add(Frozen);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 充值校园卡的命令
        /// </summary>
        public ICommand ChargeCreditCommand
        {
            get => _chargeCreditCommand;
            set => _chargeCreditCommand = value;
        }
        private ICommand _chargeCreditCommand;


        /// <summary>
        /// 挂失校园卡的命令
        /// </summary>
        public ICommand SetUpLostStateCommand { get; set; }

        /// <summary>
        /// 解挂校园卡的命令
        /// </summary>
        public ICommand CancelLostStateCommand { get; set; }
    }
}
