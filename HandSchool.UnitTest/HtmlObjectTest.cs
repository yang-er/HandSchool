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
                { "3080", "רҵ��" },
                { "3082", "����" },
                { "3084", "������" },
                { "3085", "Уѡ�޿�" },
            };

            tcmTypeSelect.FirstKeyValuePair = new KeyValuePair<string, string>("-1", "��������");
            tcmTypeSelect.OnChanged = "onchange";
            var sb = new StringBuilder();
            tcmTypeSelect.ToHtml(sb, true);
            var creation1 = sb.ToString();
            sb.Clear();

            sb.Append("<select class=\"form-control\" id=\"tcmType\" onchange=\"onchange\">")
              .Append("<option value=\"-1\" selected>��������</option>")
              .Append("<option value=\"3080\">רҵ��</option>")
              .Append("<option value=\"3082\">����</option>")
              .Append("<option value=\"3084\">������</option>")
              .Append("<option value=\"3085\">Уѡ�޿�</option>")
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
                { "�γ�����", 15 },
                { "�γ̴���", 8 },
                { "�γ�����", 6 },
                { "����ѧ��", 6 },
                { "����ѧ��", 6 },
                { "����ѧԺ", 15 },
                { "�γ�ѧ��", 6 },
                { "�γ���ѧʱ", 7.5m },
                { "��ʵ��ѧʱ", 7.5m },
            };

            progList.DefaultContent = "<tr><td colspan=\"9\">����ѡ��һ����ѧ�ƻ�</td></tr>";

            var sb = new StringBuilder();
            progList.ToHtml(sb);
            var creation1 = sb.ToString();

            var creation2 =
                "<table class=\"table table-responsive\"><thead><tr>" +
                "<th scope=\"col\" style=\"min-width:15em\">�γ�����</th>" +
                "<th scope=\"col\" style=\"min-width:8em\">�γ̴���</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">�γ�����</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">����ѧ��</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">����ѧ��</th>" +
                "<th scope=\"col\" style=\"min-width:15em\">����ѧԺ</th>" +
                "<th scope=\"col\" style=\"min-width:6em\">�γ�ѧ��</th>" +
                "<th scope=\"col\" style=\"min-width:7.5em\">�γ���ѧʱ</th>" +
                "<th scope=\"col\" style=\"min-width:7.5em\">��ʵ��ѧʱ</th>" +
                "</tr></thead><tbody id=\"progList\"><tr><td colspan=\"9\">" +
                "����ѡ��һ����ѧ�ƻ�</td></tr></tbody></table>";

            Assert.AreEqual(creation1, creation2);
        }
    }
}
