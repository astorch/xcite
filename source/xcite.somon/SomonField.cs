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

        /// <summary> Field name </summary>
        public string Name { get; }

        /// <summary> Field value </summary>
        public string Value { get; }
    }
}