namespace HandSchool.Models
{
    public class TaskResp
    {
        public TaskResp(bool isSuccess, object msg)
        {
            IsSuccess = isSuccess;
            Msg = msg;
        }

        public bool IsSuccess {get;}
        public object Msg { get; }
        public override string ToString()
        {
            return Msg.ToString();
        }
    }
}