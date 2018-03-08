using HandSchool.Internal;
using HandSchool.JLU.JsonObject;
using System;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU
{
    class GradeItem : IGradeItem
    {
        private ArchiveScoreValue asv;

        public GradeItem(ArchiveScoreValue value)
        {
            asv = value;
        }

        public string Name => asv.course.courName;
        public string Score => asv.score;
        public string Point => asv.gpoint;
        public string Credit => asv.credit;
        public bool ReSelect => asv.isReselect != "N";
        public bool Pass => asv.isPass == "Y";
        public string Term => asv.teachingTerm.termName;
        public DateTime Date => asv.dateScore;

        public string Type
        {
            get
            {
                return "";
            }
        }
        
    }

    class GradeEntrance : ISystemEntrance
    {
        private UIMS uims;
        public ISchoolSystem Parent => uims;
        private string lastReport;
        public int RowLimit = 15;

        public string Name => "成绩查询";
        public string ScriptFileUri => "service/res.do";
        public bool IsPost => true;
        public string PostValue => "{\"tag\":\"archiveScore @queryCourseScore\",\"branch\":\"latest\",\"params\":{},\"rowLimit\":" + RowLimit + "}";
        
        public void Execute()
        {
            lastReport = Parent.Post(ScriptFileUri, PostValue);
            var ro = JSON<RootObject>(lastReport);
            foreach (var asv in ro.value)
            {
                Grade.Add(new GradeItem(asv));
            }
        }
        
        public class RootObject
        {
            public string id { get; set; }
            public int status { get; set; }
            public ArchiveScoreValue[] value { get; set; }
            public string resName { get; set; }
            public string msg { get; set; }
        }
    }
}
