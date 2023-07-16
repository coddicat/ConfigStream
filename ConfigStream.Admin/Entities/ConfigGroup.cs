using ConfigStream.Abstractions;

namespace ConfigStream.Admin.Redis.Entities
{
    public class ConfigGroup : IConfigGroup
    {
        public string Name { get; set; }
    }
}