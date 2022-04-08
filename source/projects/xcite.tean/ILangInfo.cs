namespace xcite.tean {
    /// <summary> Provides information about a language. </summary>
    public interface ILangInfo {
        /// <summary> Punctuation marks </summary>
        char[] PuncMarks { get; }
        
        /// <summary> Date separator mark </summary>
        char DateSeparator { get; }
        
        /// <summary> Time separator mark </summary>
        char TimeSeparator { get; }
        
        /// <summary> Word seperator marks </summary>
        char[] WordSeparator { get; }

        /// <summary>
        /// Notifies the instance to strip any expression that represents a time statement. 
        /// Each found statement must be replaced by token that can be created using 
        /// the given <paramref name="tokenFactory"/>.
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <param name="tokenFactory">Factory to create tokens</param>
        /// <returns>Processed text</returns>
        string StripTimeExpression(string text, ITokenFactory tokenFactory);

        /// <summary>
        /// Notifies the instance to strip any expression that represents a date statement. 
        /// Each found statement must be replaced by token that can be created using 
        /// the given <paramref name="tokenFactory"/>.
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <param name="tokenFactory">Factory to create tokens</param>
        /// <returns>Processed text</returns>
        string StripDateExpression(string text, ITokenFactory tokenFactory);

        /// <summary>
        /// Notifies the instance to strip any expression that represents an abbreviation. 
        /// Each found statement must be replaced by token that can be created using 
        /// the given <paramref name="tokenFactory"/>.
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <param name="tokenFactory">Factory to create tokens</param>
        /// <returns>Processed text</returns>
        string StripAbbreviations(string text, ITokenFactory tokenFactory);
    }
}