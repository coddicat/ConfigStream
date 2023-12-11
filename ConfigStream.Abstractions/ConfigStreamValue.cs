using System;

namespace ConfigStream.Abstractions
{
    public struct ConfigStreamValue
    {
        public bool IsDefault { get; set; }
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The actual config value as a string.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Exception encountered during the process, if any
        /// </summary>
        public Exception Exception { get; set; }
    }
}
