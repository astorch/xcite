using System;

namespace xcite.nio {
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class NetServiceAttribute : Attribute {
        public string Version { get; set; } = "1.0";
    }
}