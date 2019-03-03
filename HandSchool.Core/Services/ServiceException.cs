using System;

namespace HandSchool.Services
{
    public class ServiceException : Exception
    {
        public ServiceException(string reason) : base(reason)
        {
        }

        public ServiceException(string reason, Exception inner) : base(reason, inner)
        {
        }
    }
}
