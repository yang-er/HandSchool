using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace HandSchool.Internal
{
    public interface IHtmlInput
    {
        void ToHtml(StringBuilder sb, bool full = true);
    }

    namespace HTMLs
    {
        public class FormGroup : IHtmlInput
        {
            public List<IHtmlInput> Children { get; set; } = new List<IHtmlInput>();

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                sb.Append(""/*@"<div class="form-group">
    <label for="openmode"><b>开启站点</b></label><br>
    <div class="custom-control custom-control-inline custom-radio" id="openmode">
      <input type="radio" class="custom-control-input" name="open" id="openyes" aria-describedby="openHelp" value="1" {if $openmode}checked {/if}>
      <label class="custom-control-label" for="openyes">是</label>
    </div>"*/);
                throw new NotImplementedException();
            }
        }

        public class Radio : IHtmlInput
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;

            public void ToHtml(StringBuilder sb, bool full = true)
            {
                throw new NotImplementedException();
            }
        }

        public enum InputType
        {
            text,
            password,
            email
        }

        public class Input : IHtmlInput
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

        public class Select : IHtmlInput
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
    }
}
