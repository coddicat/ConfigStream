namespace ConfigStream.Abstractions
{
    public interface IConfig
    {
        string DefaultValue { get; set; }
        string[] AllowedValues { get; set; }
    }
}
