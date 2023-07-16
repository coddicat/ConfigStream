namespace WebApplication1.ConfigurationService
{
    public class ConfigurationGroupAttribute : Attribute
    {
        public ConfigurationGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }

        public string GroupName { get; }
    }
}
