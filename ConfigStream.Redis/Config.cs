using ConfigStream.Abstractions;

namespace ConfigStream.Redis
{
    internal class Config : IConfig
    {
        public string DefaultValue { get; set; }
        public string[] AllowedValues { get; set; }
    }
}
