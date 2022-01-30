using System;

namespace HandSchool.Internal
{
    public class TimeoutManager
    {
        private DateTime? _lastRefreshTime;
        private readonly double _timeoutSec;
        public TimeoutManager(double timeoutSec)
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