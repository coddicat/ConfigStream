namespace ConfigStream.Admin.Redis.Models
{
    public class SubmitConfigValue
    {
        //public SimpleConfigValue[] Values { get; set; }
        //public string ConfigName { get; set; }
        //public string GroupName { get; set; }
        public string Value { get; set; }
        public string EnvironmentName { get; set; }
        public string TargetName { get; set; }
        public string ConfigName { get; set; }
        public string GroupName { get; set; }
    }
    //public class SimpleConfigValue
    //{
    //    public string Environment { get; set; }
    //    public string Value { get; set; }
    //    public Dictionary<string, string> TargetValues { get; set; }
    //}
}
