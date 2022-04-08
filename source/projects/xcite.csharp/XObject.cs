namespace xcite.csharp {
    /// <summary> Defines an extended C# base object class. </summary>
    public class XObject {
        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString() {
            string toString = $"{GetHashCode():X4}@{GetType().FullName}"; // ISSUE: Hash code may change
            return toString;
        }
    }
}