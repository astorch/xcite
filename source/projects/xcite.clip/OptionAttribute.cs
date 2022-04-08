using System;

namespace xcite.clip {
    /// <summary> Announces the annotated property as option with the specified name. </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute {
        /// <summary>
        /// Announces the annotated property as option with the specified <paramref name="shortName"/>.
        /// </summary>
        public OptionAttribute(char shortName) : this(shortName, null) {
            // Nothing to do here
        }

        /// <summary>
        /// Announces the annotated property as option with the specified <paramref name="fullName"/>.
        /// </summary>
        public OptionAttribute(string fullName) : this(default(char), fullName) {
            // Nothing to do here
        }

        /// <summary>
        /// Announces the annotated property as option with the specified
        /// <paramref name="shortName"/> and <paramref name="fullName"/>.
        /// </summary>
        public OptionAttribute(char shortName, string fullName) {
            ShortName = shortName;
            FullName = fullName;
        }

        /// <summary> Short name of the option </summary>
        public char ShortName { get; }

        /// <summary> Full name of the option </summary>
        public string FullName { get; }

        /// <summary> Required flag </summary>
        public bool Required { get; set; }

        /// <summary> Default value </summary>
        public object Default { get; set; }

        /// <summary> Help text </summary>
        public string HelpText { get; set; }
    }
}