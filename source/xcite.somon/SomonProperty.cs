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
            Fields = fields ?? new SomonField[0];
        }

        /// <summary> Property kind </summary>
        public string Kind { get; }

        /// <summary> (Optional) Property id. </summary>
        public string Id { get; }
        
        /// <summary> Property fields </summary>
        public SomonField[] Fields { get; }
    }
}