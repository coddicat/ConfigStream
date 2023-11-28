namespace ConfigStream.Admin.Redis.Models
{
    public class ConfigValue
    {
        public string GroupName { get; set; }
        public string ConfigName { get; set; }
        public string EnvironmentName { get; set; }
        public string TargetName { get; set; }
        public string Value { get; set; }
        public bool Deleted { get; set; }
    }
}
