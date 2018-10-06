using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Models
{
    class CurriculumItemSet
    {
        public int DayBegin = 0;
        public int DayEnd = 0;
        public static bool CompareClass(CurriculumItem A,CurriculumItem B)
        {
            if (A.Name != B.Name ||
                        A.DayBegin != B.DayBegin ||
                        A.DayEnd != B.DayEnd ||
                        A.Teacher != B.Teacher ||
                        A.Classroom != B.Classroom||
                        A.WeekBegin != B.WeekBegin
                        )
                return false;
            else
                return true;
        }
        public List<CurriculumItem> CurriculumItemList = new List<CurriculumItem>();
        public CurriculumItemSet()
        {

        }
        public CurriculumItemSet(CurriculumItem Item)
        {
            CurriculumItemList.Add(Item);
        }
        
        public static CurriculumItemSet operator +(CurriculumItemSet A, CurriculumItem B)
        {
            A.CurriculumItemList.Add(B);
            return A;
        }
        public static CurriculumItemSet operator +(CurriculumItemSet A, CurriculumItemSet B)
        {
            A.CurriculumItemList.AddRange(B.CurriculumItemList);
            return A;
        }
        public override bool Equals(object obj)
        {
            
            if (obj == null|| obj.GetType()!=GetType()|| (obj as CurriculumItemSet).CurriculumItemList.Count!=CurriculumItemList.Count)
                return false;
            else
            {
                var Temp = (obj as CurriculumItemSet).CurriculumItemList;
                for (int i=0;i<CurriculumItemList.Count;i++)
                    if (!CompareClass(Temp[i], CurriculumItemList[i]))
                        return false;
                return true;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void MargeClasses()
        {
            for(int i=0;i<CurriculumItemList.Count;i++)
            {
                for(int j=i+1;j<CurriculumItemList.Count;j++)
                {
                    if (CompareClass(CurriculumItemList[i], CurriculumItemList[j]))
                        CurriculumItemList.RemoveAt(j);
                }
            }
        }

    }
}
