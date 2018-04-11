using HandSchool.Blank;
using HandSchool.Internal;
using HandSchool.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HandSchool
{
    public partial class App : Application
    {
        private Action LoadBlank()
        {
            Service = new BlankSchool();
            DailyClassCount = 11;
            GradePoint = new GradeEntrance();
            Schedule = new Schedule();
            Message = new MessageEntrance();
            Feed = new FeedEntrance();
            return () => { };
        }
    }

    namespace Blank
    {
        class BlankSchool : NotifyPropertyChanged, ISchoolSystem
        {
            public string ServerUri => "";
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public bool IsLogin => true;
            public bool NeedLogin => false;
            public int CurrentWeek { get; set; } = 0;
            public string Tips => "";
            public bool AutoLogin { get; set; } = true;
            public bool SavePassword { get; set; } = true;
            public string WelcomeMessage => "欢迎。";
            public string CurrentMessage => "";

            public int DailyClassCount
            {
                get => App.Current.DailyClassCount;
                set => SetProperty(ref App.Current.DailyClassCount, value);
            }

            public AwaredWebClient WebClient { get; set; }
            public NameValueCollection AttachInfomation { get; set; } = new NameValueCollection();
            public event EventHandler<LoginStateEventArgs> LoginStateChanged;
            
            public async Task<string> Get(string url)
            {
                await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Get(url)"));
                return "";
            }

            public async Task<bool> Login()
            {
                await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Login()"));
                return false;
            }

            public async Task<string> Post(string url, string send)
            {
                await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->Post(url, send)"));
                return "";
            }

            public async Task<bool> RequestLogin()
            {
                await Task.Run(() => System.Diagnostics.Debug.WriteLine("Blank->RequestLogin()"));
                return false;
            }
        }

        class GradeEntrance : IGradeEntrance
        {
            public string GPAPostValue => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public string ScriptFileUri => throw new NotImplementedException();

            public bool IsPost => throw new NotImplementedException();

            public string PostValue => throw new NotImplementedException();

            public string StorageFile => throw new NotImplementedException();

            public string LastReport => throw new NotImplementedException();

            public Task Execute()
            {
                throw new NotImplementedException();
            }

            public Task<string> GatherGPA()
            {
                throw new NotImplementedException();
            }

            public void Parse()
            {
                throw new NotImplementedException();
            }
        }

        class Schedule : IScheduleEntrance
        {
            public int ClassNext => throw new NotImplementedException();

            public List<CurriculumItem> Items => throw new NotImplementedException();

            public string Name => throw new NotImplementedException();

            public string ScriptFileUri => throw new NotImplementedException();

            public bool IsPost => throw new NotImplementedException();

            public string PostValue => throw new NotImplementedException();

            public string StorageFile => throw new NotImplementedException();

            public string LastReport => throw new NotImplementedException();

            public Task Execute()
            {
                throw new NotImplementedException();
            }

            public void Parse()
            {
                throw new NotImplementedException();
            }

            public void RenderWeek(int week, Grid.IGridList<View> list, bool showAll = false)
            {
                throw new NotImplementedException();
            }

            public void Save()
            {
                throw new NotImplementedException();
            }
        }

        class MessageEntrance : IMessageEntrance
        {
            public string Name => throw new NotImplementedException();

            public string ScriptFileUri => throw new NotImplementedException();

            public bool IsPost => throw new NotImplementedException();

            public string PostValue => throw new NotImplementedException();

            public string StorageFile => throw new NotImplementedException();

            public string LastReport => throw new NotImplementedException();

            public Task Delete(int id)
            {
                throw new NotImplementedException();
            }

            public Task Execute()
            {
                throw new NotImplementedException();
            }

            public void Parse()
            {
                throw new NotImplementedException();
            }

            public Task SetReadState(int id, bool read)
            {
                throw new NotImplementedException();
            }
        }

        class FeedEntrance : IFeedEntrance
        {
            public string Name => throw new NotImplementedException();

            public string ScriptFileUri => throw new NotImplementedException();

            public bool IsPost => throw new NotImplementedException();

            public string PostValue => throw new NotImplementedException();

            public string StorageFile => throw new NotImplementedException();

            public string LastReport => throw new NotImplementedException();

            public Task Execute()
            {
                throw new NotImplementedException();
            }

            public void Parse()
            {
                throw new NotImplementedException();
            }
        }
    }
}
