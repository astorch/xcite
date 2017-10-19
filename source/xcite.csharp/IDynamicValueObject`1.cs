namespace xcite.csharp {
    /// <summary> Generic declaration of <see cref="IDynamicValueObject"/>. </summary>
    /// <typeparam name="TValue">Type of value</typeparam>
    public interface IDynamicValueObject<out TValue> : IDynamicValueObject {
        /// <summary> Observable value that is being referenced. </summary>
        new TValue Value { get; }
    }
}