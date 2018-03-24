using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace HandSchool
{
    public interface IMessageItem : INotifyPropertyChanged
    {
        int Id { get; }
        string Title { get; }
        string Body { get; }
        DateTime Time { get; }
        string Date { get; }
        string Sender { get; }
        bool Unread { get; set; }
        Command SetRead { get; }
        Command Delete { get; }
    }

    public interface IMessageEntrance : ISystemEntrance
    {
        Task SetReadState(int id, bool read);
        Task Delete(int id);
    }
}
