using System;
using System.Collections.Generic;
using System.Text;

namespace HandSchool.Internal.HtmlObject
{
    /// <summary>
    /// 复选框
    /// </summary>
    public class Check : IHtmlObject
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ValueDescription { get; set; } = string.Empty;
        public string Id { get; private set; }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            if (full) sb.Append($"<label><b>{Title}</b></label><br>");
            Id = Guid.NewGuid().ToString("N").Substring(0, 6);
            sb.Append("<div class=\"custom-control custom-control-inline custom-checkbox\">");
            sb.Append($"<input type=\"checkbox\" class=\"custom-control-input\" name=\"{Name}[]\" id=\"{Name}{Id}\" value=\"{Value}\">");
            sb.Append($"<label class=\"custom-control-label\" for=\"{Name}{Id}\">{ValueDescription}</label></div>");
            if (full && Description.Length > 0) sb.Append($"<small class=\"form-text text-muted\">{Description}</small>");
        }
    }
        
    /// <summary>
    /// 输入类型
    /// </summary>
    public enum InputType
    {
        text,
        password,
        email
    }

    /// <summary>
    /// 表示输入框
    /// </summary>
    public class Input : IHtmlObject
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public InputType Type { get; set; } = InputType.text;
        public string Placeholder { get; set; } = string.Empty;
        public string Default { get; set; } = string.Empty;
        public string Id { get; private set; }

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            Id = string.Empty;
            if (full)
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 6);
                sb.Append($"<label for=\"{Id}\"><b>{Title}</b></label><input type=\"{Type.ToString()}\" name=\"{Name}\" placeholder=\"{Placeholder}\" value=\"{Default}\" id=\"{Id}\">");
            }
            else
                sb.Append($"<input type=\"{Type.ToString()}\" name=\"{Name}\" placeholder=\"{Placeholder}\" value=\"{Default}\">");
            if (full && Description.Length > 0)
                sb.Append($"<small id=\"{Id}\" class=\"form-text text-muted\">{Description}</small>");
        }
    }
        

    /// <summary>
    /// 表示数据表
    /// </summary>
    public class Table : IHtmlObject
    {
        public string Class { get; set; } = "table table-sm";
        public List<string> Column { get; set; } = new List<string>();
        public string Id => "";
        public string BodyId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 6);

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append($"<table class=\"{Class}\"><thead><tr>");
            Column.ForEach((s) => sb.Append($"<th scope=\"col\">{s}</th>"));
            sb.Append($"</tr></thead><tbody id=\"{BodyId}\"></tbody></table>");
        }
    }
        

    /// <summary>
    /// 表示 div
    /// </summary>
    public class Div : IHtmlObject
    {
        public string Class { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append("<div");
            if (Id == string.Empty) sb.Append(" id=\"" + Id + "\"");
            if (Class == string.Empty) sb.Append(" class=\"" + Class + "\"");
            sb.Append(">");
            Children.ForEach((obj) => obj.ToHtml(sb));
            sb.Append("</div>");
        }
    }

    /// <summary>
    /// 表示 row-col
    /// </summary>
    public class Row : IHtmlObject
    {
        public List<IHtmlObject>[] Children { get; set; }
        public int[] RowWidth { get; set; }
        public string Id => "";

        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append("<div class=\"row\"" + (Id == "" ? "" : " id=\""+ Id +"\"") + ">");
            for (int i = 0; i < RowWidth.Length; i++)
            {
                sb.Append($"<div class=\"col-12 col-md-{RowWidth[i]}\">");
                Children[i].ForEach((obj) => obj.ToHtml(sb));
                sb.Append("</div>");
            }
            sb.Append("</div>");
        }
    }

    /// <summary>
    /// 表示 ul
    /// </summary>
    public class UnorderedList : IHtmlObject
    {
        public string Id => "";
        public List<string> Children { get; set; } = new List<string>();
                
        public void ToHtml(StringBuilder sb, bool full = true)
        {
            sb.Append("<ul>");
            Children.ForEach((val) => sb.Append($"<li>{val}</li>"));
            sb.Append("</ul>");
        }
    }
}
