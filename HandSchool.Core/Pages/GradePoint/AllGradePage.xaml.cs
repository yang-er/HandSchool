using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandSchool.ViewModels;
using HandSchool.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AllGradePage : ViewObject
    {
        public AllGradePage()
        {
            ViewModel = GradePointViewModel.Instance;
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                var str = ex.Message;
            }
            
        }
        private void CheckHandler(object sender, EventArgs args)
        {
            Task.Run(() =>
            {
                var l = (ViewModel as GradePointViewModel)?.AllGradeItems?.ToList();
                if (l is null) return;
                
            });
        }
    }
}