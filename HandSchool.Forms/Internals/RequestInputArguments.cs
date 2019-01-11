using System.Threading.Tasks;

namespace HandSchool.Internal
{
    public class RequestInputArguments
    {
        public RequestInputArguments()
        {
            Result = new TaskCompletionSource<string>();
        }
        
        public string Accept { get; set; }
        
        public string Cancel { get; set; }
        
        public string Message { get; set; }

        public TaskCompletionSource<string> Result { get; }
        
        public string Title { get; set; }

        public void SetResult(string result)
        {
            Result.TrySetResult(result);
        }
    }
}
