using System;

namespace HandSchool.Models
{
    /// <summary>
    /// 一个任务返回值的包装类
    /// </summary>
    public class TaskResp
    {
        /// <summary>
        /// 消息为空的“真”
        /// </summary>
        public static readonly TaskResp True = new TaskResp(true);
        
        /// <summary>
        /// 消息为空的“假”
        /// </summary>
        public static readonly TaskResp False = new TaskResp(false);
        public TaskResp(bool isSuccess, object msg = null)
        {
            IsSuccess = isSuccess;
            Msg = msg;
        }

        public bool IsSuccess {get;}
        public object Msg { get; }
        public override string ToString()
        {
            return Msg?.ToString() ?? string.Empty;
        }
    }
}