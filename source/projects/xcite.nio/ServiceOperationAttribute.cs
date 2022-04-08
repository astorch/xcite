using System;

namespace xcite.nio {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ServiceOperationAttribute : Attribute {
        public int Id { get; set; } = int.MinValue;
    }
}
