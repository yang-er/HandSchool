using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace HandSchool.UWP
{
    public sealed partial class CurriculumLabel : UserControl
    {
        public CurriculumItem Context { get; }
        public int ColorId { get; private set; }

        public CurriculumLabel(CurriculumItem value, int id)
        {
            InitializeComponent();
            Context = value;
            ColorId = id;
            Padding = new Thickness(5);
            Update();
            DoubleTapped += async (sender, args) => await ItemTapped();
        }

        public void Update()
        {
            Grid.SetColumn(this, Context.WeekDay);
            Grid.SetRow(this, Context.DayBegin);
            Grid.SetRowSpan(this, Context.DayEnd - Context.DayBegin + 1);
            Title.Text = Context.Name;
            Where.Text = Context.Classroom;
            Background = new SolidColorBrush(GetColor());
        }
        
        private async Task ItemTapped()
        {/*
            var page = new CurriculumPage(this.Context);
            await page.ShowAsync(Navigation);
        */}
        
        public Color GetColor()
        {
            // thanks to brady
            return Internal.Helper.ScheduleColors[ColorId % 8];
        }
    }
}
