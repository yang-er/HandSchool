using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HandSchool.JLU.Models
{
    internal sealed class CardBasicInfo : List<SchoolCardInfoPiece>, INotifyPropertyChanged
    {
        public SchoolCardInfoPiece NameInfo { get; set; }
        public SchoolCardInfoPiece CardId { get; set; }
        public SchoolCardInfoPiece Balance { get; set; }
        public SchoolCardInfoPiece Bank { get; set; }
        public SchoolCardInfoPiece CurGd { get; set; }
        public SchoolCardInfoPiece LastGd { get; set; }
        public SchoolCardInfoPiece Lost { get; set; }
        public SchoolCardInfoPiece Frozen { get; set; }
        public SchoolCardInfoPiece Type { get; set; }
        public SchoolCardInfoPiece Part { get; set; }

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
            Bank = new SchoolCardInfoPiece("银行卡号", "不知晓");
            CurGd = new SchoolCardInfoPiece("当前过渡余额", "听不清");
            LastGd = new SchoolCardInfoPiece("上次过渡余额", "看不见");
            Lost = new SchoolCardInfoPiece("挂失状态", "可能正常吧");
            Frozen = new SchoolCardInfoPiece("冻结状态", "大概正常吧");
            Type = new SchoolCardInfoPiece("身份类型", "你先登录");
            Part = new SchoolCardInfoPiece("部门名称", "我猜不透");

            Frozen.PropertyChanged += OnPropertyChanged;
            Lost.PropertyChanged += OnPropertyChanged;

            Add(NameInfo); Add(CardId); Add(Balance);
            Add(Bank); Add(CurGd); Add(LastGd);
            Add(Lost); Add(Frozen); Add(Type); Add(Part);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
