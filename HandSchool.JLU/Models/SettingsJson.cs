namespace HandSchool.JLU.Models
{
    internal class SettingsJson
    {
        public SettingsJson()
        {
            ProxyServer = "10.60.65.8"; // uims.jlu.edu.cn
            UseHttps = false;
            OutsideSchool = false;
        }

        public string ProxyServer { get; set; }
        public bool UseHttps { get; set; }
        public bool OutsideSchool { get; set; }
    }
}
