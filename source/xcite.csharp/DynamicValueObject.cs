namespace xcite.csharp {
    /// <summary> Provides an implementation of <see cref="IDynamicValueObject{TValue}"/>. </summary>
    /// <typeparam name="TValue">Type of value being wrapped</typeparam>
    public class DynamicValueObject<TValue> : IDynamicValueObject<TValue> {
        private TValue _value;

        /// <summary>
        /// Initialies the instance with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Initial value</param>
        public DynamicValueObject(TValue value) {
            _value = value;
        }

        /// <summary> Initializes the instance with the value type-specified default value. </summary>
        public DynamicValueObject() : this(default(TValue)) {
            // Nothing to do here
        }

        /// <inheritdoc />
        public event ObjectChangedHandler ObjectChanged;

        /// <inheritdoc />
        object IDynamicValueObject.Value 
            => Value;

        /// <inheritdoc />
        public TValue Value {
            get { return _value; }
            set {
                _value = value;
                ObjectChanged?.Invoke(this, nameof(Value));
            } 
        }
    }
}