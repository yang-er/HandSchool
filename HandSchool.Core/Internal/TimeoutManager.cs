using System;

namespace HandSchool.Internal
{
    public class TimeoutManager
    {
        private DateTime? _lastRefreshTime;
        private readonly int _timeoutSec;
        public TimeoutManager(int timeoutSec)
        {
            _timeoutSec = timeoutSec;
        }
        public bool IsTimeout()
        {
            if (_lastRefreshTime == null) return false;
            return (DateTime.Now - _lastRefreshTime.Value).TotalSeconds > _timeoutSec;
        }

        public bool NotInit => _lastRefreshTime == null;
        public void Refresh()
        {
            _lastRefreshTime = DateTime.Now;
        }
    }
}