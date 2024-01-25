using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("xcite.clip.tests")]
namespace xcite.clip {
    /// <summary> Provides the CLI parser implementation. </summary>
    public static class Parser {
        /// <summary>
        /// Disable this option to force the parser to take the arguments
        /// as provided. Otherwise it will take the arguments from the
        /// original command line invocation. The default value is TRUE.
        ///
        /// Explanation:
        /// If an CLI argument is quoted, for instance a path, and the
        /// path ends with a backshlash, Windows takes the backslash as
        /// escape character for the double quote. In this case, the
        /// arguments get messed up. The marvel mode prevents this behavior
        /// by self-parsing and splitting the CLI arguments. In this case,
        /// the args given to <see cref="Run"/> are being ignored.
        /// By disabling this flag, you can change that behavior.
        /// </summary>
        public static bool MarvelMode { get; set; } = true;

        /// <summary>
        /// Parses the given set of <paramref name="args"/> based on the information
        /// of the given <paramref name="optionTypes"/>. If the arguments are valid,
        /// the declared <paramref name="onSuccess"/> method is invoked with the result
        /// values. Returns TRUE if the arguments could be parsed.
        /// </summary>
        public static bool Run(string[] args, Action<string, object> onSuccess, params Type[] optionTypes) {
            if (optionTypes == null || optionTypes.Length == 0) throw new ArgumentNullException(nameof(optionTypes));
            if (args == null || args.Length == 0) return PrintUsageWithError("Missing arguments.", optionTypes, null);

            const char whitespaceTk = ' ';
            const char dblQuoteTk = '"';
            const char minusTk = '-';

            string argLine;

            // See explanation at property
            if (MarvelMode) {
                string originalCl = Environment.CommandLine;

                // The first part is always the way the programm has been started, for instance
                //   (i) "c:\program.exe" ...
                //  (ii) program.exe ...
                // (iii) program ...
                // The first non-whitespace character separates the first part from the rest
                int k = 0;
                bool escapeMode = false;
                char d;
                while (k != originalCl.Length && ((d = originalCl[k++]) != whitespaceTk || escapeMode)) {
                    if (d == dblQuoteTk) {
                        escapeMode = !escapeMode;
                    }
                }

                // Find the first non-whitespace character
                while (k != originalCl.Length && originalCl[k++] == whitespaceTk) {
                    // Just go ahead
                }

                argLine = originalCl.Substring(k - 1);
            } else {
                argLine = string.Join(" ", args);
            }

            if (string.IsNullOrEmpty(argLine)) return PrintUsageWithError("Missing arguments.", optionTypes, null);

            char[] argLineChars = argLine.ToCharArray();
            int maxChars = argLineChars.Length;

            // Modes
            //   (i) No-verb mode starting with -
            //  (ii) No-verb mode starting with --
            // (iii) Verb mode followed by key-value pairs

            int offset = 0;
            string verb = null;
            // We expect a verb
            if (argLineChars[0] != minusTk) {
                char[] verbBuffer = new char[100];
                int l = -1;
                char c;
                while (++l != maxChars && (c = argLineChars[l]) != whitespaceTk)
                    verbBuffer[l] = c;

                verb = new string(verbBuffer, 0, l);
                offset = l + 1;
            }

            // Verbs require at least 2 options
            bool verbMode = optionTypes.Length > 1;

            // Do we have a verb, but verb mode is no configured?
            if (verb != null && !verbMode) return PrintUsageWithError($"Verb '{verb}' is not supported.", optionTypes, null);
            if (verb == null && verbMode) return PrintUsageWithError("Verb required.", optionTypes, null);

            VerbInfo verbNfo = GetVerb(verb, optionTypes);
            if (verbNfo == null) return PrintUsageWithError($"Verb '{verb}' is unknown.", optionTypes, null);
            
            object verbInstance = verbNfo.GetVerbInstance();
            OptionInfo[] verbOptions = verbNfo.GetOptions();
            LinkedList<OptionInfo> requiredSet = new LinkedList<OptionInfo>();

            // Apply default values to options and select required
            for (int l = -1; ++l != verbOptions.Length;) {
                OptionInfo option = verbOptions[l];
                AssignValue(verbInstance, option, option.GetDefaultValue());

                if (option.IsRequired())
                    requiredSet.AddLast(option);
            }

            if (offset >= maxChars && requiredSet.Count != 0) return PrintUsageWithError($"Missing required arguments for verb '{verb}'.", optionTypes, verbNfo);

            char[] buffer = new char[1024];
            int p;
            int i = offset;
            while (i != maxChars) {
                // -(-)KEY VALUE -(-)KEY "VALUE" -(-)KEY|flag -(-)KEY VALUE ...

                char c = argLineChars[i];
                bool shortName = true;

                // Parse the key
                if (c != minusTk) return PrintUsageWithError($"Invalid syntax.", optionTypes, verbNfo);
                int j = i + 1;
                if (j == maxChars) return PrintUsageWithError($"Invalid syntax.", optionTypes, verbNfo);
                if (argLineChars[j] == minusTk) {
                    j += 1;
                    shortName = false;
                }

                i = j;
                p = 0;
                char d;
                while (i != maxChars && (d = argLineChars[i++]) != whitespaceTk) {
                    buffer[p++] = d;
                }

                string key = new string(buffer, 0, p);
                if (string.IsNullOrEmpty(key)) return PrintUsageWithError($"Invalid syntax.", optionTypes, verbNfo);

                // Get the option info for that key
                OptionInfo optionNfo = GetOption(verbOptions, key, shortName);
                if (optionNfo == null) return PrintUsageWithError($"Option '{key}' is not supported for that verb.", optionTypes, verbNfo);

                // Boolean keys do not require a value
                if (optionNfo.IsBool()) {
                    AssignValue(verbInstance, optionNfo, true);
                    requiredSet.Remove(optionNfo);
                    continue;
                }

                // Parse the value
                if (i == maxChars) return PrintUsageWithError($"Invalid syntax.", optionTypes, verbNfo);

                d = argLineChars[i];
                if (d == minusTk) return PrintUsageWithError($"Invalid syntax.", optionTypes, verbNfo);

                bool escapeMode = false;
                p = 0;
                while (i != maxChars && ((d = argLineChars[i++]) != whitespaceTk || escapeMode)) {
                    if (d == dblQuoteTk) {
                        escapeMode = !escapeMode;
                        continue; // Strip
                    }

                    buffer[p++] = d;
                }

                string value = new string(buffer, 0, p);
                AssignValue(verbInstance, optionNfo, value);
                requiredSet.Remove(optionNfo);
            }

            // All required options must be set
            if (requiredSet.Count != 0) {
                OptionInfo missReqOpt = requiredSet.First.Value;
                return PrintUsageWithError($"Missing value for required option '{missReqOpt.GetFullName()}'.", optionTypes, verbNfo);
            }

            onSuccess(verb, verbInstance);
            return true;
        }

        /// <summary>
        /// Writes the help (usage) information to the console standard stream for
        /// the given set of <paramref name="optionTypes"/>.
        /// </summary>
        public static void PrintUsage(Type[] optionTypes) {
            PrintUsage(Console.Out, optionTypes);
        }

        /// <summary>
        /// Writes the help (usage) information to the given <paramref name="stream"/> for
        /// the given set of <paramref name="optionTypes"/>.
        /// </summary>
        public static void PrintUsage(TextWriter stream, Type[] optionTypes) {
            PrintUsageWithError(stream, null, optionTypes, null);
        }

        /// <summary>
        /// Writes the help (usage) information including the specified <paramref name="errorText"/>
        /// to the console standard stream for the given set of <paramref name="optionTypes"/>.
        /// Returns always FALSE for fluent signature.
        /// </summary>
        private static bool PrintUsageWithError(string errorText, Type[] optionTypes, VerbInfo verbNfo) {
            return PrintUsageWithError(Console.Out, errorText, optionTypes, verbNfo);
        }

        /// <summary>
        /// Writes the help (usage) information including the specified <paramref name="errorText"/>
        /// to the assigned <paramref name="stream"/> for the given set of <paramref name="optionTypes"/>.
        /// Note, the <paramref name="errorText"/> is only written when it's not NULL.
        /// Returns always FALSE for fluent signature.
        /// </summary>
        private static bool PrintUsageWithError(TextWriter stream, string errorText, Type[] optionTypes, VerbInfo verbNfo) {
            if (stream == null) return false;
            const int padRight = 50;
            const string leftOffset = "  ";
            const string midOffset = "     ";

            Assembly myAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            FileVersionInfo fileVersionNfo = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            string name = fileVersionNfo.ProductName;
            string version = fileVersionNfo.ProductVersion;
            string copyright = fileVersionNfo.LegalCopyright;
            
            stream.WriteLine($"{name} {version}");
            stream.WriteLine(copyright);
            stream.WriteLine();
            
            if (!string.IsNullOrEmpty(errorText)) {
                stream.WriteLine("ERROR(S)");
                stream.WriteLine($"  {errorText}");
                stream.WriteLine();
            }

            stream.WriteLine("ARGUMENT(S)");

            // No-verb mode?
            if (verbNfo == null && optionTypes.Length == 1) {
                verbNfo = GetVerb(optionTypes[0], null);
            }

            // Print verb information
            if (verbNfo == null) {
                int maxVerbNameChars = 0;
                VerbInfo[] verbSet = new VerbInfo[optionTypes.Length];
                int p = 0;
                for (int i = -1; ++i != optionTypes.Length;) {
                    Type optionType = optionTypes[i];
                    VerbAttribute verbAttr = optionType.GetCustomAttribute<VerbAttribute>();
                    if (verbAttr == null) continue;
                    VerbInfo optVerbNfo = GetVerb(optionType, verbAttr);
                    maxVerbNameChars = Math.Max(maxVerbNameChars, optVerbNfo.GetName().Length);
                    verbSet[p++] = optVerbNfo;
                }

                Array.Resize(ref verbSet, p);

                for (int i = -1; ++i != p;) {
                    VerbInfo verbOpt = verbSet[i];
                    string fullline = string.Concat(
                        leftOffset,
                        verbOpt.GetName().PadLeft(maxVerbNameChars),
                        midOffset,
                        verbOpt.GetHelpText().PadRight(padRight)
                    );

                    stream.WriteLine(fullline);
                    stream.WriteLine();
                }

                stream.WriteLine("Unknown action. Check help!");
                return false;
            }
            
            // Print option information
            OptionInfo[] options = verbNfo.GetOptions();
            string[] headlines = new string[options.Length];
            string[] bodylines = new string[options.Length];
            int maxOptionNameChars = 0;
            for (int i = -1, ilen = options.Length; ++i != ilen;) {
                OptionInfo option = options[i];
                string shortName = option.GetShortName();
                string fullName = option.GetFullName();
                string required = option.IsRequired() ? "(Required) " : string.Empty;
                string deflt = option.GetDefaultValue() != null ? $"(Default {option.GetDefaultValue()}) " : string.Empty;
                string helpText = option.GetHelpText();

                bool hasShortName = !string.IsNullOrEmpty(shortName);
                bool hasFullName = !string.IsNullOrEmpty(fullName);

                string headline = string.Empty;

                if (hasShortName)
                    headline = string.Concat(headline, "-", shortName);

                if (hasShortName && hasFullName)
                    headline = string.Concat(headline, ", ");

                if (hasFullName)
                    headline = string.Concat(headline, "--", fullName);

                maxOptionNameChars = Math.Max(maxOptionNameChars, headline.Length);
                headlines[i] = headline;

                string bodyline = string.Concat(required, deflt, helpText);
                bodylines[i] = bodyline;
            }
            
            for (int i = -1, ilen =headlines.Length; ++i != ilen;) {
                string headline = headlines[i];
                string bodyline = bodylines[i];
                string fullline = string.Concat(
                    leftOffset,
                    headline.PadLeft(maxOptionNameChars),
                    midOffset,
                    bodyline.PadRight(padRight)
                );

                string[] fulllineParts = fullline.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                stream.WriteLine(fulllineParts[0]);

                if (fulllineParts.Length > 1) {
                    int wsCount = leftOffset.Length + maxOptionNameChars + midOffset.Length;
                    string leadingWhitespaces = new string(' ', wsCount);
                    for (int j = 0, jlen = fulllineParts.Length; ++j != jlen;) {
                        string fulllinePart = fulllineParts[j];
                        string shiftedPart = string.Concat(leadingWhitespaces, fulllinePart);
                        stream.WriteLine(shiftedPart);
                    }
                }

                stream.WriteLine();
            }

            stream.WriteLine("Missing arguments. Check help!");
            return false;
        }

        /// <summary>
        /// Assigns the given <paramref name="value"/> to the <paramref name="verbInstance"/>
        /// for the given <paramref name="option"/>.
        /// </summary>
        private static void AssignValue(object verbInstance, OptionInfo option, object value) {
            PropertyInfo property = option.GetProperty();
            Type propertyType = property.PropertyType;
            object typeSafeValue = value;

            if (typeSafeValue != null && propertyType.IsEnum) {
                typeSafeValue = Enum.Parse(propertyType, typeSafeValue.ToString(), true);
            }

            if (typeSafeValue != null && propertyType != typeSafeValue.GetType())
                typeSafeValue = Convert.ChangeType(typeSafeValue, propertyType);

            property.SetValue(verbInstance, typeSafeValue);
        }

        /// <summary>
        /// Returns the option information of the verb option from the defined
        /// <paramref name="options"/> set that matches the specified <paramref name="name"/>.
        /// When <paramref name="shortName"/> is set, the look-up uses the short option names.
        /// Returns NULL if no match is found.
        /// </summary>
        private static OptionInfo GetOption(OptionInfo[] options, string name, bool shortName) {
            if (options == null) return null;
            for (int i = -1; ++i != options.Length;) {
                OptionInfo option = options[i];
                string optionName = shortName ? option.GetShortName() : option.GetFullName();
                if (!string.Equals(name, optionName, StringComparison.InvariantCultureIgnoreCase)) continue;
                return option;
            }

            return null;
        }

        /// <summary>
        /// Returns the verb information of the given <paramref name="verb"/> that may
        /// match an element of the given <paramref name="optionTypes"/>. If no matching
        /// element is found, NULL is returned.
        /// </summary>
        private static VerbInfo GetVerb(string verb, Type[] optionTypes) {
            if (verb == null) {
                return GetVerb(optionTypes[0], null);
            }

            for (int i = -1; ++i != optionTypes.Length;) {
                Type optionType = optionTypes[i];
                VerbAttribute verbAttr = optionType.GetCustomAttribute<VerbAttribute>();
                if (verbAttr == null) continue;
                if (!string.Equals(verb, verbAttr.Name, StringComparison.InvariantCultureIgnoreCase)) continue;
                return GetVerb(optionType, verbAttr);
            }

            return null;
        }

        /// <summary> Return the verb information of the given <paramref name="optionType"/>. </summary>
        private static VerbInfo GetVerb(Type optionType, VerbAttribute verb) {
            PropertyInfo[] optionPropertySet = optionType.GetProperties();
            OptionInfo[] optionSet = new OptionInfo[optionPropertySet.Length];
            int p = 0;
            for (int i = -1; ++i != optionPropertySet.Length;) {
                PropertyInfo property = optionPropertySet[i];
                OptionAttribute optAttr = property.GetCustomAttribute<OptionAttribute>();
                if (optAttr == null) continue;
                optionSet[p++] = new OptionInfo(property, optAttr);
            }

            Array.Resize(ref optionSet, p);
            return new VerbInfo(optionType, verb, optionSet);
        }
    }
}
