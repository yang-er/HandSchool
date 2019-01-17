using System.Threading.Tasks;

namespace HandSchool.Internal
{
    public class RequestAlertArguments
    {
        public RequestAlertArguments(string title, string message, string accept, string cancel)
        {
            Title = title;
            Message = message;
            Accept = accept;
            Cancel = cancel;
            Result = new TaskCompletionSource<bool>();
        }
        
        public string Accept { get; }
        
        public string Cancel { get; }
        
        public string Message { get; }

        public TaskCompletionSource<bool> Result { get; }
        
        public string Title { get; }

        public void SetResult(bool result)
        {
            Result.TrySetResult(result);
        }
    }
}
