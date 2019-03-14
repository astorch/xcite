using System.Reflection;

namespace xcite.clip {
    /// <summary>
    /// Provides information about a property that has been annotated
    /// with an <see cref="OptionAttribute"/>.
    /// </summary>
    class OptionInfo {
        private readonly OptionAttribute _attribute;
        private readonly PropertyInfo _property;

        /// <inheritdoc />
        public OptionInfo(PropertyInfo property, OptionAttribute attribute) {
            _property = property;
            _attribute = attribute;
        }

        /// <summary> Returns the full name value of the option. </summary>
        public string GetFullName() {
            return _attribute.FullName;
        }

        /// <summary> Returns the short name value of the option. </summary>
        public string GetShortName() {
            return new string(_attribute.ShortName, 1) ;
        }

        /// <summary> Returns TRUE if the option means a boolean flag. </summary>
        public bool IsBool() {
            return _property.PropertyType == typeof(bool);
        }

        /// <summary> Returns TRUE if the option is declared as required. </summary>
        public bool IsRequired() {
            return _attribute.Required;
        }

        /// <summary> Returns the assigned default option value. </summary>
        public object GetDefaultValue() {
            return _attribute.Default;
        }

        /// <summary> Returns the option help text. </summary>
        public string GetHelpText() {
            return _attribute.HelpText;
        }

        /// <summary> Returns the property that is annotated as option. </summary>
        public PropertyInfo GetProperty() {
            return _property;
        }
    }
}