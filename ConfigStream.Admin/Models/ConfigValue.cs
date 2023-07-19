﻿namespace ConfigStream.Admin.Redis.Models
{
    public class ConfigValue
    {
        public Dictionary<string, string> EnvironmentValues { get; set; }
        public string ConfigName { get; set; }
        public string GroupName { get; set; }
        public string[] AllowedValues { get; set; }
        public string DefaultValue { get; set; }
    }
}
