namespace ConfigStream.Admin.Redis.Models
{
    public class SubmitConfigValue
    {
        public Dictionary<string, string> EnvironmentValues { get; set; }
        public string ConfigName { get; set; }
        public string GroupName { get; set; }
    }
}
