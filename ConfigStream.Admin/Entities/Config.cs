using ConfigStream.Abstractions;

namespace ConfigStream.Admin.Redis.Entities
{
    public class Config : IConfig
    {
        public string? ConfigName { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public string[]? AllowedValues { get; set; }
        public string? DefaultValue { get; set; }        
    }
}