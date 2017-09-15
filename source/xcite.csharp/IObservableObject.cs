namespace xcite.csharp {
    /// <summary>
    /// Signature of a handler for the <see cref="IObservableObject.ObjectChanged"/> event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="propertyName">Name of the changed property</param>
    public delegate void ObjectChangedHandler(object sender, string propertyName);

    /// <summary> Object that raises an event when an owned property has been changed. </summary>
    public interface IObservableObject {
        /// <summary> Is raises when an owned property has been changed. </summary>
        event ObjectChangedHandler ObjectChanged;
    }
}