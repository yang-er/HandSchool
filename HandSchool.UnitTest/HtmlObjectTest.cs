using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HandSchool.Internal.HtmlObject;
using HandSchool.Internal;

namespace HandSchool.UnitTest
{
    [TestClass]
    public class HtmlObjectTest
    {
        [TestMethod]
        public void SelectTestMethod()
        {
            var tcmTypeSelect = new Select("tcmType")
            {
                { "3080", "专业课" },
                { "3082", "重修" },
                { "3084", "体育课" },
                { "3085", "校选修课" },
            };

            tcmTypeSelect.FirstKeyValuePair = new KeyValuePair<string, string>("-1", "任意类型");
            tcmTypeSelect.OnChanged = "onchange";
            var sb = new StringBuilder();
            tcmTypeSelect.ToHtml(sb, true);
            var creation1 = sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"tcmType\" onchange=\"onchange\">")
              .Append("<option value=\"-1\" selected>任意类型</option>")
              .Append("<option value=\"3080\">专业课</option>")
              .Append("<option value=\"3082\">重修</option>")
              .Append("<option value=\"3084\">体育课</option>")
              .Append("<option value=\"3085\">校选修课</option>")
              .Append("</select>");
            var creation2 = sb.ToString();
            sb.Clear();

            Assert.AreEqual(creation1, creation2);
        }

        private IEnumerable<KeyValuePair<string, string>> CreateSampleData()
        {
            for (int i = 0; i < 10; i++)
            {
                var guid = Guid.NewGuid();
                yield return new KeyValuePair<string, string>(guid.ToString().Substring(0, 6), guid.ToString());
            }
        }

        [TestMethod]
        public void EnumerableAdapterTestMethod()
        {
            var adapted = new EnumerableAdapter(CreateSampleData());
            var sb = new StringBuilder();
            foreach (var pair in adapted)
                sb.AppendLine(pair.Key + "=" + pair.Value);
            Assert.AreNotEqual(sb.ToString(), "");
        }

        [TestMethod]
        public void TableResponsiveTestMethod()
        {
            var progList = new TableResponsive(bodyId: "progList")
            {
                { "课程名称", 15 },
                { "课程代码", 8 },
                { "课程性质", 6 },
                { "建议学年", 6 },
                { "建议学期", 6 },
                { "开课学院", 15 },
                { "课程学分", 6 },
                { "课程总学时", 7.5m },
                { "内实践学时", 7.5m },
            };

            progList.DefaultContent = "<tr><td colspan=\"9\">请先选择一个教学计划</td></tr>";

            var sb = new StringBuilder();
            progList.ToHtml(sb);
            var creation1 = sb.ToString();

            var creation2 =
                "<table class=\"table table-responsive\"><thead><tr>" +
                "<th scope=\"col\" style=\"min-width:15em\">课程名称</th>" +
                "<th scope=\"col\" style=\"min-width:8em\">课程代码</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">课程性质</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">建议学年</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">建议学期</th>" +
                "<th scope=\"col\" style=\"min-width:15em\">开课学院</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">课程学分</th>" +
                "<th scope=\"col\" style=\"min-width:7.5em\">课程总学时</th>" +
                "<th scope=\"col\" style=\"min-width:7.5em\">内实践学时</th>" +
                "</tr></thead><tbody id=\"progList\"><tr><td colspan=\"9\">" +
                "请先选择一个教学计划</td></tr></tbody></table>";

            Assert.AreEqual(creation1, creation2);
        }
    }
}
