using System;

namespace xcite.clip {
    /// <summary> Announces the annotated type as verb with the specified name. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VerbAttribute : Attribute {
        /// <summary>
        /// Announces the annotated type as verb with the specified <paramref name="name"/>.
        /// </summary>
        public VerbAttribute(string name) {
            Name = name;
        }

        /// <summary> CLI name of the verb </summary>
        public string Name { get; }

        /// <summary> Verb help text </summary>
        public string HelpText { get; set; }
    }
}