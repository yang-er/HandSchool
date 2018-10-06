using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandSchool.Models;
using Xamarin.Forms;

namespace HandSchool.Models
{
    class CurriculumLabelSet:StackLayout
    {
        public int ColorId { get; private set; }
        public static string[] WeekOenString = new string[3] { "\n双周", "\n单周", "\n" };
        public CurriculumLabelSet(CurriculumItemSet curriculumItemSet)
        {
            Grid.SetRow(this,curriculumItemSet.DayBegin);
            Padding = new Thickness(1);
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            WidthRequest = 1000;//为什么设置LayoutOptions不能水平占满？？
            var temp = new List<CurriculumItem>(curriculumItemSet.CurriculumItemList);
            temp.Sort((a,b)=> { return a.WeekBegin .CompareTo( b.WeekBegin);  });
            foreach (var item in temp)
            {
                AddItem(new CurriculumLabel(item,item.Name[0]));
            }
            for(int i=0;i<Children.Count-1;i++)
            {

                int Index = -1;
                for (int j = i+1; j < Children.Count; j++)
                    if ((Children[i] as Label).FormattedText.Spans[0].Text == (Children[j] as Label).FormattedText.Spans[0].Text)
                    {
                        Index = j;
                        break;
                    }

                if (Index!=-1)
                {
                    (Children[i] as Label).FormattedText.Spans[1].Text += (Children[Index] as Label).FormattedText.Spans[1].Text;
                    Children.RemoveAt(Index);
                    i--;
                }
            }
        }

        public void AddItem(CurriculumLabel NewItem)
        {
            if(Children.Count==0)//统一背景颜色
            {
                BackgroundColor = NewItem.BackgroundColor;
                Grid.SetColumn(this, Grid.GetColumn(NewItem));
                Grid.SetRowSpan(this, Grid.GetRowSpan(NewItem));
            }
            else
            {
                NewItem.BackgroundColor = BackgroundColor;
                Grid.SetRowSpan(this, Math.Min(Grid.GetRowSpan(NewItem), Grid.GetRowSpan(this)));
            }
            
            Children.Add(new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FormattedText = new FormattedString { Spans = { NewItem.Title, new Span { Text=$"{WeekOenString[(int)NewItem.Context.WeekOen]}{NewItem.Context.WeekBegin}周-{NewItem.Context.WeekEnd}周", ForegroundColor = Color.FromRgba(255, 255, 255, 220) } } },
                VerticalOptions = HorizontalOptions = LayoutOptions.CenterAndExpand
            });
            
        }
    }
}
