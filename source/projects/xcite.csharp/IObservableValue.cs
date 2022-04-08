using System;

namespace xcite.csharp {
    /// <summary>
    /// Declares a structure that contains a single value which may be change from time to time. 
    /// However, if the value changes a value changed event is raised.
    /// </summary>
    public interface IObservableValue {
        /// <summary> Is raises when the value has been changed. </summary>
        event EventHandler ValueChanged;

        /// <summary> Observable value that is being referenced. </summary>
        object Value { get; }
    }
}