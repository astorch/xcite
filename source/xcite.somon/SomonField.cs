namespace xcite.somon {
    /// <summary> Describes a SOMON object property field. </summary>
    public class SomonField {
        /// <summary> Initializes the new instance. </summary>
        /// <param name="name">Field name</param>
        /// <param name="value">Field value</param>
        public SomonField(string name, string value) {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}