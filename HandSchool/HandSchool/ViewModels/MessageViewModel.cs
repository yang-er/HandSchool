using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using static HandSchool.Internal.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandSchool.ViewModels
{
    /*
#error Need to be deleted
    public class StartPageMsg:INotifyPropertyChanged
    {
        public string TeacherStr { get; set; }
        public string AddressStr { get; set; }
        public string WelcomeStr { get; set; }
        public string WeekStr { get; set; }
        public string LastGetWheatherTime { get; set; }
        public string NextClassStr { get; set; }
        public AwaredWebClient WebClient { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public NameValueCollection AttachInfomation { get; set; }
        public string ChineseWeekday ="一二三四五六七";
        public Command ReflushCommand { get; set; }
        public StartPageMsg()
        {
            WebClient = new AwaredWebClient("https://www.sojson.com/open/api/weather/json.shtml?city=", Encoding.UTF8);
            WelcomeStr = $"{App.Current.Service.WelcomeMessage}";
            WeekStr = $"第{ChineseWeekday[App.Current.Service.CurrentWeek]}周";
            GetNextClass();
            ReflushCommand = new Command(async() => await ExecuteReflushCommand());
        }
        async Task ExecuteReflushCommand()
        {
            GetNextClass();
            WelcomeStr = $"{ App.Current.Service.WelcomeMessage}";
            WeekStr = $"第{ChineseWeekday[App.Current.Service.CurrentWeek]}周";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void GetNextClass()//获取下一节课
        {
            int DayOfWeek;
            int Classnext;
            if (App.Current.Schedule.ClassNext==11)
            {
                DayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
                Classnext = 1;
            }
            else
            {
                Classnext = App.Current.Schedule.ClassNext;
                DayOfWeek = (int)DateTime.Now.DayOfWeek;
            }
            foreach (var i in App.Current.Schedule.Items)
            {
                if ((int)i.WeekOen == 0 && App.Current.Service.CurrentWeek % 2 == 0) continue;
                if ((int)i.WeekOen == 1 && App.Current.Service.CurrentWeek % 2 == 1) continue;
                if (i.WeekDay <DayOfWeek)
                { }
                else if(i.WeekDay ==DayOfWeek)
                {
                    if(i.DayBegin>=Classnext)
                    {
                        NextClassStr = $"下节课是:{i.Name}";
                        TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}"; AddressStr = $"{i.Classroom}";
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                        return;
                    }
                }
                else if(i.WeekDay > DayOfWeek)
                {
                    NextClassStr = $"下节课是:{i.Name}";
                    TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}";
                    AddressStr = $"{i.Classroom}";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                    return;
                }
            }
            foreach (var i in App.Current.Schedule.Items)
            {
                if ((int)i.WeekOen == 0 && App.Current.Service.CurrentWeek % 2 == 0) continue;
                if ((int)i.WeekOen == 1 && App.Current.Service.CurrentWeek % 2 == 1) continue;
                Debug.Print($"{i.WeekDay}\n");
                if(i.WeekDay== DayOfWeek)break;
                if(i.WeekDay< DayOfWeek)
                {
                    NextClassStr = $"下节课是:{i.Name}";
                    TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}";
                    AddressStr = $"{i.Classroom}";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));

                    return;
                }   
            }
            NextClassStr = "无(无课程或未刷新)";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));

            return; 
        }
    }
    */
    public class MessageViewModel : BaseViewModel
    {
        public ObservableCollection<IMessageItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        static MessageViewModel instance = null;
        
        public static MessageViewModel Instance
        {
            get
            {
                if (instance is null) instance = new MessageViewModel();
                return instance;
            }
        }

        public MessageViewModel()
        {
            Title = "站内消息";
            Items = new ObservableCollection<IMessageItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }
        
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await App.Current.Message.Execute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
