namespace ConfigStream.Admin.Redis.Models
{
    public class SubmitConfigValue
    {
        public required string ConfigName { get; set; }
        public required string GroupName { get; set; }
        public required string EnvironmentName { get; set; }
        public string? TargetName { get; set; }

        /// <summary>
        /// If null or empty then delete from storage
        /// </summary>
        public string? Value { get; set; }
    }
}
