using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;

namespace HandSchool.Internal
{
    public interface IHtmlObject
    {
        void ToHtml(StringBuilder sb, bool full = true);
    }

    namespace HTMLs
    {
        public class Form : IHtmlObject
        {
            public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();
            public string SubmitOption { get; set; } = "return false";

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                sb.Append($"<form class=\"setting-form-group\" onsubmit=\"{SubmitOption}\">");
                Children.ForEach((obj) => obj.ToHtml(sb));
                sb.Append("</form>");
                throw new NotImplementedException();
            }
        }

        public class FormGroup : IHtmlObject
        {
            public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                sb.Append("<div class=\"form-group\">");
                Children.ForEach((obj) => obj.ToHtml(sb));
                sb.Append("</div>");
            }
        }

        public class Check : IHtmlObject
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string ValueDescription { get; set; } = string.Empty;

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                if (full) sb.Append($"<label><b>{Title}</b></label><br>");
                var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
                sb.Append("<div class=\"custom-control custom-control-inline custom-checkbox\">");
                sb.Append($"<input type=\"checkbox\" class=\"custom-control-input\" name=\"{Name}[]\" id=\"{Name}{guid}\" value=\"{Value}\">");
                sb.Append($"<label class=\"custom-control-label\" for=\"{Name}{guid}\">{ValueDescription}</label></div>");
                if (full && Description.Length > 0) sb.Append($"<small class=\"form-text text-muted\">{Description}</small>");
            }
        }

        public class Radio : IHtmlObject
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public NameValueCollection Options { get; set; } = new NameValueCollection();

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                if (full) sb.Append($"<label><b>{Title}</b></label><br>");
                foreach (var key in Options.AllKeys)
                {
                    var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
                    sb.Append("<div class=\"custom-control custom-control-inline custom-radio\">");
                    sb.Append($"<input type=\"radio\" class=\"custom-control-input\" name=\"{Name}\" id=\"{Name}{guid}\" value=\"{key}\">");
                    sb.Append($"<label class=\"custom-control-label\" for=\"{Name}{guid}\">{Options["key"]}</label></div>");
                }
                if (full && Description.Length > 0) sb.Append($"<small class=\"form-text text-muted\">{Description}</small>");
            }
        }

        public enum InputType
        {
            text,
            password,
            email
        }

        public class Input : IHtmlObject
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public InputType Type { get; set; } = InputType.text;
            public string Placeholder { get; set; } = string.Empty;
            public string Default { get; set; } = string.Empty;

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                var id = string.Empty;
                if (full)
                {
                    id = Guid.NewGuid().ToString("N").Substring(0, 6);
                    sb.Append($"<label for=\"{id}\"><b>{Title}</b></label><input type=\"{Type.ToString()}\" name=\"{Name}\" placeholder=\"{Placeholder}\" value=\"{Default}\" id=\"{id}\">");
                }
                else
                    sb.Append($"<input type=\"{Type.ToString()}\" name=\"{Name}\" placeholder=\"{Placeholder}\" value=\"{Default}\">");
                if (full && Description.Length > 0)
                    sb.Append($"<small id=\"{id}\" class=\"form-text text-muted\">{Description}</small>");
            }
        }

        public class Select : IHtmlObject
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public NameValueCollection Options { get; set; } = new NameValueCollection();

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                var id = string.Empty;
                if (full)
                {
                    id = Guid.NewGuid().ToString("N").Substring(0, 6);
                    sb.Append($"<label for=\"{id}\"><b>{Title}</b></label><select class=\"form-control\" name=\"{Name}\" id=\"{id}\">");
                }
                else
                    sb.Append($"<select class=\"form-control\" name=\"{Name}\">");
                foreach (var key in Options?.AllKeys)
                    sb.Append($"<option value=\"{key}\">{Options[key]}</option>");
                sb.Append("</select>");
                if (full && Description.Length > 0)
                    sb.Append($"<small id=\"{id}\" class=\"form-text text-muted\">{Description}</small>");
            }
        }

        public class Table : IHtmlObject
        {
            public string Class { get; set; } = "table table-sm";
            public List<string> Column { get; set; } = new List<string>();
            public string BodyId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 6);

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                sb.Append($"<table class=\"{Class}\"><thead><tr>");
                Column.ForEach((s) => sb.Append($"<th scope=\"col\">{s}</th>"));
                sb.Append($"</tr></thead><tbody id=\"{BodyId}\"></tbody></table>");
            }
        }

        public class Button : IHtmlObject
        {
            public string Title { get; set; } = " 提交 ";
            public string Color { get; set; } = "primary";

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                if (full) sb.Append("<div class=\"form-group\">");
                sb.Append($"<button type=\"submit\" class=\"btn btn-{Color}\">{Title}</button>");
                if (full) sb.Append("</div>");
            }
        }

        public class Bootstrap : IHtmlObject
        {
            public string Title { get; set; } = "WebViewPage";
            public const string Charset = "utf-8";
            public List<IHtmlObject> Children { get; set; } = new List<IHtmlObject>();

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                sb.Append($"<!doctype html><html><head><meta charset=\"{Charset}\"><base href=\"{{webview_base_url}}\"></base>");
                sb.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1, shrink-to-fit=no\">");
                sb.Append($"<link rel=\"stylesheet\" href=\"bootstrap.css\"><title>{Title}</title></head><body>");
                Children.ForEach((obj) => obj.ToHtml(sb));
                sb.Append("<script src=\"jquery.js\"></script><script src=\"popper.js\"></script><script src=\"json.js\"></script>");
                sb.Append("<script src=\"bootstrap.bundle.js\"></script></body></html>");
            }
        }
    }
}
