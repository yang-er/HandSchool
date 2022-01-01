using System.Collections.Generic;

namespace HandSchool.JLU.JsonObject
{
    public class LibRoomData
    {
        public string id { get; set; }
        public string accno { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string msn { get; set; }
        public string ident { get; set; }
        public string dept { get; set; }
        public string deptid { get; set; }
        public string tutor { get; set; }
        public string tutorid { get; set; }
        public string cls { get; set; }
        public string clsid { get; set; }
        public bool receive { get; set; }
        public string tsta { get; set; }
        public string rtsta { get; set; }
        public string pro { get; set; }
        public int score { get; set; }
        public List<List<string>> credit { get; set; }
        public string role { get; set; }
    }

    public class LibRoomJson
    {
        public int ret { get; set; }
        public string act { get; set; }
        public string msg { get; set; }
        public LibRoomData data { get; set; }
        public string ext { get; set; }
    }
}