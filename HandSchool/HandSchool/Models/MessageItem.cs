using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace HandSchool
{
    namespace Models
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
            Command SetUnread { get; }
            Command Delete { get; }
        }
    }

    namespace Services
    {
        public interface IMessageEntrance : ISystemEntrance
        {
            Task SetReadState(int id, bool read);
            Task Delete(int id);
        }
    }
}
