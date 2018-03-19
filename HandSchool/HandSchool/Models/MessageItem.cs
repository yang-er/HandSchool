using System;

namespace HandSchool
{
    public interface IMessageItem
    {
        int Id { get; }
        string Title { get; }
        string Body { get; }
        DateTime Time { get; }
        bool Readed { get; }
        string Show { get; }
    }
}
