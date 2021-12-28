using HandSchool.Internal;
using HandSchool.Internals;
using HandSchool.JLU.ViewModels;
using HandSchool.Models;
using HandSchool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.JLU.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.JLU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeacherEva
    {
        public TeacherEva()
        {
            InitializeComponent();
            ViewModel = TeacherEvaVM.Instance;
            if (TeacherEvaVM.Instance.IsEmpty)
            {
                Task.Run(TeacherEvaVM.Instance.GetEvaItems);
            }
        }
    }

    [Entrance("JLU", "一键教评", "好评未评价的老师们", EntranceType.InfoEntrance)]

    public class TeacherEvaShell : ITapEntrace
    {
        public async Task Action(INavigate navigate)
        {
            await navigate.PushAsync(typeof(TeacherEva), null);
        }
    }
}