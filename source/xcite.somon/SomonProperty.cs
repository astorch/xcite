namespace xcite.somon {
    /// <summary> Describes a property of a SOMON object. </summary>
    public class SomonProperty {
        
        /// <summary> Initializes the new instance. </summary>
        /// <param name="kind">Kind of property</param>
        /// <param name="id">Property id</param>
        /// <param name="fields">Fields of the property</param>
        public SomonProperty(string kind, string id, SomonField[] fields) {
            Kind = kind;
            Id = id;
            Fields = fields;
        }

        public string Kind { get; }

        public string Id { get; }
        
        public SomonField[] Fields { get; }
    }
}