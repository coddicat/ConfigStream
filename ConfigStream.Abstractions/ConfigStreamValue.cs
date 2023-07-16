using System;

namespace ConfigStream.Abstractions
{
    public struct ConfigStreamValue
    {
        public bool IsDefault { get; set; }
        public bool IsSuccess { get; set; }
        public string Data { get; set; }
        public Exception Exception { get; set; }
    }
}
