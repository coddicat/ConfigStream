namespace WebApplication1.ConfigurationService
{
    public class AllowedValuesAttribute : Attribute
    {
        public AllowedValuesAttribute(string[] allowedValues) 
        {
            Values = allowedValues;
        }
        public string[] Values { get; }
    }
}
