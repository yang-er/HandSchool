using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HandSchool.ViewModels
{
    /// <summary>
    /// 基本的控制器，实现了H5交互的基类。
    /// </summary>
    /// <inheritdoc cref="BaseViewModel" />
    /// <inheritdoc cref="IWebEntrance" />
    public abstract class BaseController : BaseViewModel, IWebEntrance
    {
        public Action<string> Evaluate { get; set; }
        public List<InfoEntranceMenu> Menu { get; } = new List<InfoEntranceMenu>();
        public abstract Task Receive(string data);
    }
}
