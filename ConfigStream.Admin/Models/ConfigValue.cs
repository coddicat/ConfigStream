using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigStream.Admin.Redis.Models
{
    public class ConfigValue
    {
        public string ConfigName { get; set; }
        public string GroupName { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
    }
}
