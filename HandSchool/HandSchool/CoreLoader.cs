using HandSchool.Services;
using System.Collections.Generic;

namespace HandSchool
{
    public sealed partial class Core
    {
        /// <summary>
        /// 可用学校列表
        /// </summary>
        public static List<ISchoolWrapper> Schools { get; } = new List<ISchoolWrapper>();

        /// <summary>
        /// 列出所有的学校
        /// </summary>
        private static void ListSchools()
        {
            Schools.Clear();
            Schools.Add(new Blank.Loader());
            Schools.Add(new JLU.Loader());
        }
    }
}
