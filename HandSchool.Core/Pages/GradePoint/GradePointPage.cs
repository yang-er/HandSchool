using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Views
{
    public class GradePointPage : IViewPresenter
    {
        public IViewPage[] GetAllPages()
        {
            return new IViewPage[]
            {
                new NewGradePage(),
                new AllGradePage()
            };
        }

        public int PageCount => 2;
        public string Title => "学分成绩";
    }
}
