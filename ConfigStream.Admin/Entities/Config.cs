using ConfigStream.Abstractions;

namespace ConfigStream.Admin.Redis.Entities
{
    public class Config : IConfig
    {
        public required string ConfigName { get; set; }
        public required string GroupName { get; set; }
        public string? Description { get; set; }
        public string[]? AllowedValues { get; set; }
        public string? DefaultValue { get; set; }        
    }
}