using HandSchool.Internal;
using HandSchool.Internal.HtmlObject;
using HandSchool.JLU.JsonObject;
using HandSchool.Models;
using HandSchool.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static HandSchool.Internal.Helper;

namespace HandSchool.JLU.InfoQuery
{
    class TeachEvaluate : IInfoEntrance
    {
        const string evalList = "{\"tag\":\"student@evalItem\",\"branch\":\"self\",\"params\":{\"blank\":\"Y\"}}";

        public string Description => throw new NotImplementedException();

        public List<string> TableHeader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Bootstrap HtmlDocument { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IViewResponse Binding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<string> Evaluate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<InfoEntranceMenu> Menu { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => throw new NotImplementedException();

        public string ScriptFileUri => throw new NotImplementedException();

        public bool IsPost => throw new NotImplementedException();

        public string PostValue => throw new NotImplementedException();

        public string StorageFile => throw new NotImplementedException();

        public string LastReport { get; private set; }

        public async Task Execute()
        {
            LastReport = await Core.App.Service.Post("service/res.do", evalList);
            var fetched_list = JSON<RootObject<StudEval>>(LastReport);
            foreach (var op in fetched_list.value)
            {
                if (op.evalActTime.evalGuideline.evalGuidelineId == "120")
                {

                }
            }

            throw new NotImplementedException();
        }

        public void Parse() { }

        public void Receive(string data)
        {
            throw new NotImplementedException();
        }
    }
}
