namespace xcite.csharp {
    /// <summary>
    /// Declares an object that contains a single value which may be changed. 
    /// However, if the value changes a <see cref="IObservableObject.ObjectChanged"/> 
    /// event is raised.
    /// </summary>
    public interface IDynamicValueObject : IObservableObject {
        /// <summary> Observable value that is being referenced. </summary>
        object Value { get; }
    }
}