using HandSchool.JLU.Models;
using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.JLU.ViewModels
{
    class YktViewModel : BaseViewModel
    {
        public static YktViewModel Instance { get; private set; }
        public ObservableCollection<PickCardInfo> PickCardInfo { get; set; }
        public SchoolCardInfo BasicInfo { get; set; }

        public Command LoadPickCardInfoCommand { get; set; }
        public Command ChargeCreditCommand { get; set; }
        public Command RecordFindCommand { get; set; }

        public YktViewModel()
        {
            System.Diagnostics.Debug.Assert(Instance is null);
            Instance = this;
            PickCardInfo = new ObservableCollection<PickCardInfo>();
            BasicInfo = new SchoolCardInfo();
        }
    }
}
