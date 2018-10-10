using System;
using System.Collections.Generic;
using System.Text;
using HandSchool.Models;
namespace HandSchool.Models
{
    class ClassTableController
    {
        public List<List<CurriculumItemSet2>> CurriculumItemGrid=new List<List<CurriculumItemSet2>>();
        public ClassTableController()
        {
            for(int i=0;i<7;i++)
            {
                List<CurriculumItemSet2> Temp = new List<CurriculumItemSet2>();
                for(int j=0;j<Core.App.DailyClassCount;j++)
                {
                    Temp.Add(new CurriculumItemSet2());
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
            foreach (var DayList in CurriculumItemGrid)
                foreach (var ClassSet in DayList)
                    ClassSet.MergeClasses();
            var EmptyClass = new CurriculumItem();
            foreach(var DayList in CurriculumItemGrid)
            {
                for (int i = 0; i < DayList.Count - 1; i++)
                    if (DayList[i] == DayList[i + 1] || DayList[i + 1].DayEnd==0)
                    {
                        DayList.RemoveAt(i + 1);
                        i--;
                    }
                if (DayList[0].DayEnd == 0)
                    DayList.RemoveAt(0);
            }

        }
        public List<CurriculumItemSet2> ToList()
        {
            MargeClassSet();
            List<CurriculumItemSet2> Temp = new List<CurriculumItemSet2>();
            foreach (var ItemList in CurriculumItemGrid)
                foreach (var Item in ItemList)
                    Temp.Add(Item);
            return Temp;
        }
    }
}
