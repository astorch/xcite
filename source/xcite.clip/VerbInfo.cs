using System;

namespace xcite.clip {
    /// <summary>
    /// Provides information about a class type that has been annotated with
    /// an <see cref="VerbAttribute"/>.
    /// </summary>
    class VerbInfo {
        private readonly Type _optionType;
        private readonly VerbAttribute _attribute;
        private readonly OptionInfo[] _options;

        /// <inheritdoc />
        public VerbInfo(Type optionType, VerbAttribute attribute, OptionInfo[] options) {
            _optionType = optionType;
            _attribute = attribute;
            _options = options;
        }

        /// <summary> Returns the name of the verb. </summary>
        public string GetName() {
            return _attribute.Name;
        }

        /// <summary> Returns the verb help text. </summary>
        public string GetHelpText() {
            return _attribute.HelpText;
        }

        /// <summary> Returns the verb options. </summary>
        public OptionInfo[] GetOptions() {
            return _options;
        }

        /// <summary>
        /// Creates a new instance of the type that has been annotated
        /// with the <see cref="OptionAttribute"/>.
        /// </summary>
        public object GetVerbInstance() {
            return Activator.CreateInstance(_optionType);
        }
    }
}