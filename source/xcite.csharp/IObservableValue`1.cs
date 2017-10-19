using System;

namespace xcite.csharp {
    /// <summary> Generic declaration of <see cref="IObservableValue"/>. </summary>
    /// <typeparam name="TValue">Type of value</typeparam>
    public interface IObservableValue<out TValue> : IObservableValue where TValue : IConvertible {
        /// <summary> Observable value that is being referenced. </summary>
        new TValue Value { get; }
    }
}