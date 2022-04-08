namespace xcite.tdn {
    public class TdnProperty {
        
        public TdnProperty(string name, ETdnElementType elementType, ETdnDataType dataType, object value) {
            Name = name;
            ElementType = elementType;
            DataType = dataType;
            Value = value;
        }

        public string Name { get; }

        public ETdnDataType DataType { get; }

        public ETdnElementType ElementType { get; }

        public object Value { get; }

        public T GetValue<T>() {
            return default(T);
        }
    }
}