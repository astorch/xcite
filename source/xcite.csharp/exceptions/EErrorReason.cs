namespace xcite.csharp.exceptions {
    /// <summary>
    /// Defines the base class of an error reason used by the <see cref="Xception{T}"/> class.
    /// </summary>
    public abstract class EErrorReason : XEnum<EErrorReason> {
        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="name">Name of the enum value</param>
        /// <param name="code">Error code</param>
        /// <param name="hint">Additional error hint</param>
        protected EErrorReason(string name, int code, string hint = null) : base(name) {
            Hint = hint;
            Code = code;
        }

        /// <summary> Returns the 3 letter code of this error reason class. </summary>
        public abstract string TLC { get; }

        /// <summary> Returns the declared error hint of this error reason. </summary>
        public string Hint { get; }

        /// <summary> Returns the declared error code of this error reason. </summary>
        public int Code { get; }

        /// <summary>
        /// Implements the explicit cast from <see cref="EErrorReason"/> to <see cref="int"/>. 
        /// If <paramref name="errorReason"/> is NULL, 0 is returned.
        /// </summary>
        /// <param name="errorReason">Instance to cast</param>
        /// <returns>Integer value</returns>
        public static explicit operator int(EErrorReason errorReason) {
            return (errorReason == null ? 0 : errorReason.Code);
        }
    }
}