namespace ConfigStream.Redis
{
    public static class RedisKeys
    {
        private const string ConfigSteam = "ConfigSteam";
        private static string Configs => $"{ConfigSteam}:Configs";
        private static string ConfigValues => $"{ConfigSteam}:Values";
        private static string Target(string targetName) => targetName is null ? "" : $":{targetName}";

        public static string Config(string groupName, string configName) => $"{Configs}:{groupName}:{configName}";        
        public static string EnvironmentTargetValue(string environment, string groupName, string configName, string targetName = null) =>
            $"{ConfigValues}:Environments:{environment}:{groupName}:{configName}{Target(targetName)}";
        public static string ScopeValue(string scopeId, string groupName, string configName, string targetName = null) =>
            $"{ConfigValues}:Scopes:{scopeId}:{groupName}:{configName}{Target(targetName)}";
    }
}
