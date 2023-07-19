using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigStream.Redis
{
    public static class RedisKeys
    {
        private const string ConfigSteam = "ConfigSteam";
        private const string Groups = "Groups";
        private const string Configs = "Configs";
        private const string Targets = "Targets";
        private const string Environments = "Environments";
        private const string Keys = "Keys";
        private const string List = "List";

        #region Entities List
        private static string GroupList => $"{ConfigSteam}:{Groups}:{List}";
        private static string ConfigList => $"{ConfigSteam}:{Configs}:{List}";
        private static string TargetList => $"{ConfigSteam}:{Targets}:{List}";
        private static string EnvironmentList => $"{ConfigSteam}:{Environments}:{List}";
        #endregion

        #region Keys of Entities
        public static string GroupKeys => $"{ConfigSteam}:{Groups}:{Keys}";
        public static string TargetKeys => $"{ConfigSteam}:{Targets}:{Keys}";
        public static string EnvironmentKeys => $"{ConfigSteam}:{Environments}:{Keys}";
        #endregion

        private static string Values => $"{ConfigSteam}:Values";


        public static string ConfigKeys(string groupName) => $"{ConfigSteam}:{Configs}:{Keys}:{groupName}";
        public static string Group(string groupName) => $"{GroupList}:{groupName}";
        public static string Config(string groupName, string configName) => $"{ConfigList}:{groupName}:{configName}";
        public static string Target(string targetName) => $"{TargetList}:{targetName}";
        public static string Environment(string environmentName) => $"{EnvironmentList}:{environmentName}";
        public static string EnvironmentValue(string environment, string groupName, string configName, string targetName = null) =>
            $"{Values}:Environments:{environment}:{groupName}:{configName}" + (targetName is null ? "" : $":{targetName}");
        public static string ScopeValue(string scopeId, string groupName, string configName, string targetName = null) =>
            $"{Values}:Scopes:{scopeId}:{groupName}:{configName}" + (targetName is null ? "" : $":{targetName}");
    }
}
