namespace HandSchool
{
    public sealed partial class Core
    {
        /// <summary>
        /// 列出所有的学校
        /// </summary>
        static void ListSchools()
        {
            Schools.Clear();
            Schools.Add(new Blank.Loader());
            Schools.Add(new JLU.Loader());
        }
    }
}
