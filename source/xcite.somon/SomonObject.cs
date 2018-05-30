namespace xcite.somon {
    /// <summary>
    /// Describes a SOMON object. Each SOMON object may contain
    /// zero or more properties.
    /// </summary>
    public class SomonObject {
        /// <summary> Initializes the new instance. </summary>
        /// <param name="properties">Properties of the object</param>
        public SomonObject(SomonProperty[] properties) {
            Properties = properties ?? new SomonProperty[0];
        }

        public SomonProperty[] Properties { get; }
    }
}