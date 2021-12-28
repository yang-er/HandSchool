using System;

namespace HandSchool.JLU.JsonObject
{
    public class StudentName
    {
        public string name { get; set; }
    }
    
    public class StudEval
    {
        public object personInput { get; set; }
        public object subject { get; set; }
        public Target target { get; set; }
        public string evalItemId { get; set; }
        public EvalActTime evalActTime { get; set; }
        public string dateInput { get; set; }
        public TargetClar targetClar { get; set; }
        public class TargetClar
        {
            public string notes { get; set; }
        }
        public class Target
        {
            public School school { get; set; }
            public string name { get; set; }
            public string personId { get; set; }

            public class School
            {
                public string schoolName { get; set; }
            }
        }

        public class EvalActTime
        {
            public string actTimeId { get; set; }
            public EvalGuideLine evalGuideline { get; set; }
            public EvalTime evalTime { get; set; }
            public EvalActivity evalActivity { get; set; }
        }

        public class EvalGuideLine
        {
            public string evalGuidelineId { get; set; }
            public string paperUrl { get; set; }
            public string displayMode { get; set; }
        }

        public class EvalTime
        {
            public string title { get; set; }
            public string phaseType { get; set; }
            public string activeStatus { get; set; }
            public string etimeId { get; set; }
            public Ttl ttl { get; set; }
        }

        public class Ttl
        {
            public string ttlId { get; set; }
            public DateTime dateStop { get; set; }
            public string title { get; set; }
            public DateTime dateStart { get; set; }
        }

        public class EvalActivity
        {
            public string title { get; set; }
        }
    }

}