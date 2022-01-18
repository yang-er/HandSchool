using SQLite;

namespace HandSchool.Models
{
    [Table("user_accounts")]
    public class UserAccount
    {
        [PrimaryKey] public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotNull] public bool AutoLogin { get; set; } = false;

        public override string ToString()
        {
            return $"ServerName: {ServerName}, UserName: {UserName}";
        }
    }
    
    [Table("server_jsons")]
    public class ServerJson
    {
        [PrimaryKey]
        public string JsonName { get; set; }
        public string Json { get; set; }
    }
}