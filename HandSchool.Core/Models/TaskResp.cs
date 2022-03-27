using System;

namespace HandSchool.Models
{
    /// <summary>
    /// 一个任务返回值的包装类
    /// </summary>
    public readonly struct TaskResp
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

        public static implicit operator bool(TaskResp taskResp)
        {
            return taskResp.IsSuccess;
        }

        public static implicit operator TaskResp(bool boolean)
        {
            return boolean ? True : False;
        }
    }
}