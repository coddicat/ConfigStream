using System.ComponentModel;

namespace WebApplication1.ConfigurationService
{
    public enum ConfigurationGroup
    {
        Infra,
        Bills,
        Platform,
        Payment
    }
    [ConfigurationGroup(nameof(ConfigurationGroup.Infra))]
    public enum InfraConfiguration
    {
        [DefaultValue("off"), Description("My Config 1"), AllowedValues(new[] { "on", "off" })]
        Config1,
        [DefaultValue("off"), Description("My Config 2"), AllowedValues(new[] { "on", "off" })]
        Config2,
        [DefaultValue("false"), Description("My Feature 1"), AllowedValues(new[] { "true", "false" })]
        Feature1,
        Feature2
    }

    [ConfigurationGroup(nameof(ConfigurationGroup.Platform))]
    public enum PlatformConfiguration
    {
        [DefaultValue("off"), Description("My Config"), AllowedValues(new[] { "on", "off" })]
        Config1,
        [AllowedValues(new[] { "true", "false" })]
        Config2,
        Feature1,
        Feature2
    }
}
