using System.Runtime.CompilerServices;

namespace xcite.csharp {
    /// <summary> Abstract implementation of <see cref="IObservableObject"/>. </summary>
    public abstract class AbstractObservableObject : IObservableObject {
        /// <inheritdoc />
        public event ObjectChangedHandler ObjectChanged;

        /// <summary>
        /// Raises the <see cref="ObjectChanged"/> event for the property or method that invokes 
        /// this method.
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        protected virtual void OnObjectChanged([CallerMemberName] string propertyName = null) {
            RaiseObjectChanged(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="ObjectChanged"/> event for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">Name of the changed property</param>
        protected virtual void RaiseObjectChanged(string propertyName) {
            ObjectChanged?.Invoke(this, propertyName);
        }
    }
}