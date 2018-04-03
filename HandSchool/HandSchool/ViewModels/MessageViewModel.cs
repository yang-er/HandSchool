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

namespace HandSchool.ViewModels
{
    public class StartPageMsg
    {
        public string TeacherStr { get; set; }
        public string AddressStr { get; set; }
        public string WelcomeStr { get; set; }
        public string WeekStr { get; set; }
        public string NextClassStr { get; set; }
        HandSchool.JLU.UIMS MessageGeter = new HandSchool.JLU.UIMS();
        HandSchool.JLU.Schedule NextClassGeter = new HandSchool.JLU.Schedule();
        public NameValueCollection AttachInfomation { get; set; }
        public string ChineseWeekday ="一二三四五六七";
        public StartPageMsg()
        {

            WelcomeStr = $"欢迎，{MessageGeter.AttachInfomation["studName"]}。";
            
            WeekStr = $"第{ChineseWeekday[MessageGeter.CurrentWeek]}周";
            GetNextClass();

        }
        public void GetNextClass()//获取下一节课
        {
            int DayOfWeek;
            if(NextClassGeter.Classnext==11)
            {
                DayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
                NextClassGeter.Classnext = 1;
            }
            else
            {
                DayOfWeek = (int)DateTime.Now.DayOfWeek;
            }
            foreach (var i in NextClassGeter.Items)
            {
                if ((int)i.WeekOen == 0 && MessageGeter.CurrentWeek % 2 == 0) continue;
                if ((int)i.WeekOen == 1 && MessageGeter.CurrentWeek % 2 == 1) continue;
                if (i.WeekDay <DayOfWeek)
                 { }
                else if(i.WeekDay ==DayOfWeek)
                {
                    if(i.DayBegin>=NextClassGeter.Classnext)
                    {
                        NextClassStr = $"下节课是:{i.Name}";
                        TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}"; AddressStr = $"{i.Classroom}";
                        return;
                    }
                }
                else if(i.WeekDay > DayOfWeek)
                {
                    NextClassStr = $"下节课是:{i.Name}";
                    TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}";
                    AddressStr = $"{i.Classroom}";
                    return;
                }
            }
            foreach (var i in NextClassGeter.Items)
            {
                if ((int)i.WeekOen == 0 && MessageGeter.CurrentWeek % 2 == 0) continue;
                if ((int)i.WeekOen == 1 && MessageGeter.CurrentWeek % 2 == 1) continue;
                Debug.Print($"{i.WeekDay}\n");
                if(i.WeekDay== DayOfWeek)break;
                if(i.WeekDay< DayOfWeek)
                {
                    NextClassStr = $"下节课是:{i.Name}";
                    TeacherStr = $"周{ChineseWeekday[i.WeekDay - 1]} {i.DayBegin}-{i.DayEnd} {i.Teacher}";
                    AddressStr = $"{i.Classroom}";
                    return;
                }   
            }
            NextClassStr = "无(无课程或未刷新)";
            return;



        }
    }
    public class Test: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
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
