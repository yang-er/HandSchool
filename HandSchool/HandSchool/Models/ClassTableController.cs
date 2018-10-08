using System;
using System.Collections.Generic;
using System.Text;
using HandSchool.Models;
namespace HandSchool.Models
{
    class ClassTableController
    {
        public List<List<CurriculumItemSet>> CurriculumItemGrid=new List<List<CurriculumItemSet>>();
        public ClassTableController()
        {
            for(int i=0;i<7;i++)
            {
                List<CurriculumItemSet> Temp = new List<CurriculumItemSet>();
                for(int j=0;j<Core.App.DailyClassCount;j++)
                {
                    Temp.Add(new CurriculumItemSet());
                }
                CurriculumItemGrid.Add(Temp);
            }
        }
        public void AddClass(CurriculumItem NewItem)
        {

            for(int i=NewItem.DayBegin-1;i<NewItem.DayEnd;i++)
            {
                CurriculumItemGrid[NewItem.WeekDay - 1][i] += NewItem;
                CurriculumItemGrid[NewItem.WeekDay - 1][i] .DayBegin=i+1;
            }
                
        }
        public void MargeClassSet()
        {

            var EmptyClass = new CurriculumItem();
            foreach(var DayList in CurriculumItemGrid)
            {
                for (int i = 0; i < DayList.Count - 1; i++)
                    if (DayList[i] == DayList[i + 1] || DayList[i + 1].DayBegin==0)
                    {
                        DayList.RemoveAt(i + 1);
                        i--;
                    }
                if (DayList[0].DayBegin==0)
                    DayList.RemoveAt(0);
            }
 
            foreach (var DayList in CurriculumItemGrid)
                foreach (var ClassSet in DayList)
                    ClassSet.MergeClasses();
            
        }

    }
}
