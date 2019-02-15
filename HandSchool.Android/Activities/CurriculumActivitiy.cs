using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HandSchool.Droid.Fragments;
using HandSchool.Internals;
using HandSchool.Models;
using HandSchool.ViewModels;
using Xamarin.Forms;

namespace HandSchool.Droid.Activities
{
    [Activity(Theme = "@style/AppTheme.NoActionBar")]
    public class CurriculumActivitiy : BaseActivity
    {
        CurriculumFragment Fragment;
        CurriculumItem a;
        public ScheduleViewModel ViewModel { get; set; }
        protected override void OnNavigatedParameter(object obj)
        {
            Fragment = obj as CurriculumFragment;
            TransactionV3(Fragment, Fragment);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ContentViewResource = Resource.Layout.activity_curriculum;
            base.OnCreate(savedInstanceState);
            var bar = SupportActionBar;
            bar.SetDisplayHomeAsUpEnabled(true);
            bar.SetHomeButtonEnabled(true);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var detail = menu.Add("创建");
            detail.SetShowAsAction(ShowAsAction.Always);
            detail.SetOnMenuItemClickListener(new MenuEntryClickedListener(new CommandAction(OnFinishCreate)));
            return base.OnCreateOptionsMenu(menu);
        }
        public  Task  OnFinishCreate()
        {
            ScheduleViewModel.Instance.SaveToFile();
            Finish();
            return Task.CompletedTask;
        }
        public override void Finish()
        {
            base.Finish();
        }
    }
}