using System;

namespace xcite.csharp {
    /// <summary> Provides an implementation of <see cref="IObservableValue{TValue}"/>. </summary>
    /// <typeparam name="TValue">Type of value being wrapped</typeparam>
    public class ObservableValue<TValue> : IObservableValue<TValue> where TValue : IConvertible {
        private TValue _value;

        /// <summary>
        /// Initialies the instance with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Initial value</param>
        public ObservableValue(TValue value) {
            _value = value;
        }

        /// <summary> Initializes the instance with the value type-specified default value. </summary>
        public ObservableValue() : this(default(TValue)) {
            // Nothing to do here
        }

        /// <inheritdoc />
        public event EventHandler ValueChanged;

        /// <inheritdoc />
        object IObservableValue.Value 
            => Value;

        /// <inheritdoc />
        public TValue Value {
            get { return _value; }
            set {
                _value = value;
                ValueChanged?.Invoke(this, EventArgs.Empty);
            } 
        }
    }
}