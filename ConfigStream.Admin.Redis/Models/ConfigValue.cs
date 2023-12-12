namespace ConfigStream.Admin.Redis.Models
{
    public class ConfigValue
    {
        public required string GroupName { get; set; }
        public required string ConfigName { get; set; }
        public required string EnvironmentName { get; set; }
        public string? TargetName { get; set; }
        public required string Value { get; set; }
    }
}
