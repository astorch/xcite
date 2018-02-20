namespace xcite.tean {
    /// <summary> Provides information about a language. </summary>
    public interface ILangInfo {
        /// <summary> Punctuation marks </summary>
        char[] PuncMarks { get; }
        
        /// <summary> Date separator mark </summary>
        char DateSeparator { get; }
        
        /// <summary> Time separator mark </summary>
        char TimeSeparator { get; }
    }
}