using HandSchool.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CurriculumPage : PopContentPage
	{
		public CurriculumPage(CurriculumLabel item)
		{
			InitializeComponent();
            BindingContext = item.Context;
            saveButton.Command = new Command(async () =>
            {
                item.Update();
                App.Current.Schedule.Save();
                await Close();
            });
		}
	}
}