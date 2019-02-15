using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using HandSchool.Droid.Activities;
using HandSchool.Models;
using HandSchool.Views;
using HandSchool.Internals;
using Jaredrummler.MaterialSpinner;
using HandSchool.ViewModels;
using Android.Support.Design.Widget;
using Android.Text;

namespace HandSchool.Droid.Fragments
{
    public class CurriculumFragment : ViewFragment, ICurriculumPage
    {
        public CurriculumItem Model{ get; set; }
        public bool IsCreate;
        [BindView(Resource.Id.startnum)]
        public MaterialSpinner StartNum { get; set; }


        [BindView(Resource.Id.startweek)]
        public MaterialSpinner StartWeek { get; set; }

        [BindView(Resource.Id.endweek)]
        public MaterialSpinner EndWeek { get; set; }

        [BindView(Resource.Id.endnum)]
        public MaterialSpinner EndNum { get; set; }

        [BindView(Resource.Id.weekoen)]
        public MaterialSpinner WeekOen { get; set; }

        [BindView(Resource.Id.weekday)]
        public MaterialSpinner Weekday { get; set; }

        [BindView(Resource.Id.teacher)]
        public TextInputEditText Teacher { get; set; }

        [BindView(Resource.Id.classroom)]
        public TextInputEditText ClassRoom { get; set; }

        [BindView(Resource.Id.classname)]
        public TextInputEditText ClassName { get; set; }
        public MaterialSpinnerAdapter StartNumAdapter;
        public MaterialSpinnerAdapter EndNumAdapter;
        public MaterialSpinnerAdapter StartWeekAdapter;
        public MaterialSpinnerAdapter EndWeekAdapter;
        public CurriculumFragment()
        {
            FragmentViewResource = Resource.Layout.curriculumlayout;
            var context = PlatformImplV2.Instance.ContextStack.Peek();
            StartNumAdapter = new MaterialSpinnerAdapter(context, new List<string>());
            EndNumAdapter = new MaterialSpinnerAdapter(context, new List<string>());
            StartWeekAdapter= new MaterialSpinnerAdapter(context, new List<string>());
            EndWeekAdapter = new MaterialSpinnerAdapter(context, new List<string>());
            for (int i = 1; i < Core.App.DailyClassCount; i++)
            {
                StartNumAdapter.Items.Add("第" + i.ToString() + "节");
                EndNumAdapter.Items.Add("第" + i.ToString() + "节");
            }
            for (int i = 1; i < 23; i++) 
            {
                StartWeekAdapter.Items.Add("第" + i.ToString() + "周");
                EndWeekAdapter.Items.Add("第" + i.ToString() + "周");
            }
           

        }
        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            //StartNum.SelectedIndex;

            StartNum.SetAdapter(StartNumAdapter);

            StartNum.ItemSelected+=OnSpinnerItemSelected;
            EndNum.ItemSelected += OnSpinnerItemSelected;

            StartWeek.ItemSelected += OnSpinnerItemSelected;
            EndWeek.ItemSelected += OnSpinnerItemSelected;

            WeekOen.ItemSelected += OnSpinnerItemSelected;
            Weekday.ItemSelected += OnSpinnerItemSelected;

            EndNum.SetAdapter(EndNumAdapter);
            StartWeek.SetAdapter(StartWeekAdapter);
            EndWeek.SetAdapter(EndWeekAdapter);
            Weekday.SetItems("星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天");
            WeekOen.SetItems("单周", "双周", "单双周");

            ClassName.AfterTextChanged += OnTextChanged;
            ClassRoom.AfterTextChanged += OnTextChanged;
            Teacher.AfterTextChanged += OnTextChanged;
            if (IsCreate == false)
            {
                StartNum.SelectedIndex = Model.DayBegin - 1;
                EndNum.SelectedIndex = Model.DayEnd - 1;
                StartWeek.SelectedIndex = Model.WeekBegin - 1;
                EndWeek.SelectedIndex = Model.WeekEnd - 1;
                WeekOen.SelectedIndex = (int)Model.WeekOen;
                Weekday.SelectedIndex = Model.WeekDay;
            }

        }
        public void OnTextChanged(object Sender, AfterTextChangedEventArgs Args)
        {
            if(Sender==ClassRoom)
            {
                Model.Classroom = (Sender as TextInputEditText).Text;
            }
            if (Sender == ClassName)
            {
                Model.Name = (Sender as TextInputEditText).Text;
            }
            if (Sender == Teacher)
            {
                Model.Teacher = (Sender as TextInputEditText).Text;
            }
        }
        public void OnSpinnerItemSelected(object Sender, MaterialSpinner.ItemSelectedEventArgs Args)
        {
            if(Sender==StartNum)
            {
                if (EndNum.SelectedIndex < StartNum.SelectedIndex)
                    EndNum.SelectedIndex = StartNum.SelectedIndex;
                Model.DayBegin = StartNum.SelectedIndex+1;
                Model.DayEnd = EndNum.SelectedIndex+1;
            }
            if (Sender == EndNum)
            {
                if (EndNum.SelectedIndex < StartNum.SelectedIndex)
                    StartNum.SelectedIndex = EndNum.SelectedIndex;
                Model.DayBegin = StartNum.SelectedIndex+1;
                Model.DayEnd = EndNum.SelectedIndex+1;
            }
            if (Sender == StartWeek)
            {
                if (EndWeek.SelectedIndex < StartWeek.SelectedIndex)
                    EndWeek.SelectedIndex = StartWeek.SelectedIndex;
                Model.WeekBegin = StartWeek.SelectedIndex + 1;
                Model.WeekEnd = EndWeek.SelectedIndex + 1;
            }
              
            if (Sender == EndWeek)
            {
                if (EndWeek.SelectedIndex < StartWeek.SelectedIndex)
                    StartWeek.SelectedIndex = EndWeek.SelectedIndex;
                Model.WeekBegin = StartWeek.SelectedIndex + 1;
                Model.WeekEnd = EndWeek.SelectedIndex + 1;
            }
            if(Sender==WeekOen)
            {
                Model.WeekOen = (WeekOddEvenNone)WeekOen.SelectedIndex;
            }
            if(Sender==Weekday)
            {
                Model.WeekDay = Weekday.SelectedIndex+1;
            }
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public void SetNavigationArguments(CurriculumItem item, bool isCreate)
        {
            Model = item;
            
            IsCreate = isCreate;
            if(isCreate)
                ScheduleViewModel.Instance.AddItem(Model);
        }

        public async Task<bool> ShowAsync()
        {
            var Result =new  TaskCompletionSource<bool>();
            var context = PlatformImplV2.Instance.ContextStack.Peek();
            var navigate = context as INavigate;
            await navigate.PushAsync<CurriculumActivitiy>(this);
            return Result.Task;
        }
    }
}