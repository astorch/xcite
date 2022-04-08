namespace xcite.tean {
    /// <summary> Factory to create tokens. </summary>
    public interface ITokenFactory {
        /// <summary> Creates a token for the specified <paramref name="value"/>. </summary>
        /// <param name="value">Value for the token</param>
        /// <returns>Token</returns>
        string Consume(string value);
    }
}