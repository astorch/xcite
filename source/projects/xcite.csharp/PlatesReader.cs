﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace xcite.csharp {
    /// <summary> Reads configuration settings written in plain text format (PLAin TExt Settings).
    /// The resolved settings are assigned to a specified object.
    /// </summary>
    public class PlatesReader {
        /// <summary>
        /// Reads the configuration from a file referenced by the given <paramref name="filePath"/> and
        /// applies each found value to a newly created instance of the given type <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="filePath">Path to the file that contains the configuration</param>
        /// <typeparam name="TObject">Type of object the settings are assigned to</typeparam>
        /// <returns>Instance of <typeparamref name="TObject"/> the configation is assigned to</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="filePath"/> is NULL or empty</exception>
        /// <exception cref="InvalidOperationException">If the referenced file does not exist</exception>
        public TObject ReadFile<TObject>(string filePath) where TObject : new() {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            string fullFilePath = Path.GetFullPath(filePath);
            if (!File.Exists(fullFilePath)) throw new InvalidOperationException("There is no file under the given path: " + fullFilePath);

            string[] textLines = File.ReadAllLines(filePath);
            return Read<TObject>(textLines);
        }

        /// <summary>
        /// Reads the configuration given by the specified <paramref name="configText"/> and applies each found value
        /// to a newly created instance of the given type <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="configText">Text that describes the configuration</param>
        /// <typeparam name="TObject">Type of object the settings are assigned to</typeparam>
        /// <returns>Instance of <typeparamref name="TObject"/> the configation is assigned to</returns>
        public TObject ReadText<TObject>(string configText) where TObject : new() {
            string[] textLines = configText.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            return Read<TObject>(textLines);
        }

        /// <summary>
        /// Reads each of the specified <paramref name="textLines"/> and applies each found value
        /// to a newly created instance of the given type <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="textLines">Set of configuration lines</param>
        /// <typeparam name="TObject">Type of object the settings are assigned to</typeparam>
        /// <returns>Instance of <typeparamref name="TObject"/> the configation is assigned to</returns>
        /// <exception cref="InvalidOperationException">If the configuration reference a property that could not be found</exception>
        public TObject Read<TObject>(string[] textLines) where TObject : new() {
            TObject obj = new TObject();
            PropertyInfo[] objPropSet = typeof(TObject).GetTypeInfo().GetProperties();
            if (objPropSet.Length == 0) return obj;
            
            for (int i = -1; ++i != textLines.Length;) {
                string textLine = textLines[i];
                
                // Ignore comments
                if (textLine.StartsWith("#")) continue;
                
                // Process assignment
                string[] assign = textLine.Split('=');
                string key = assign[0].Trim();
                
                // Empty lines aren't relevant
                if (string.IsNullOrEmpty(key)) continue;
                
                // Look up property
                string value = assign[1].Trim();
                PropertyInfo property = GetObjectProperty(key, objPropSet);
                
                // Path couldn't be resolved
                if (property == null) throw new InvalidOperationException($"Could not resolve path '{key}' for the given object '{typeof(TObject)}'.");

                bool isNullable = Nullable.GetUnderlyingType(property.PropertyType) != null;
                bool isEnum = property.PropertyType.IsEnum;

                object clrValue;
                if (isNullable) {
                    clrValue = ToNullableValue(property, value);
                } else if (isEnum) {
                    clrValue = ToClrEnumValue(property, value);
                } else {
                    clrValue = ToTargetType(property.PropertyType, value);
                }

                property.SetValue(obj, clrValue);
            }

            return obj;
        }

        /// <summary>
        /// Returns the object property that matches the specified <paramref name="name"/>. If the
        /// name declares a property path, it's resolved also. If no property could be resolved,
        /// NULL is returned.
        /// </summary>
        /// <param name="name">Name (or path) of the property to resolve</param>
        /// <param name="properties">Basic set of property to look up</param>
        /// <returns>Property with the specified name or NULL</returns>
        private PropertyInfo GetObjectProperty(string name, PropertyInfo[] properties) {
            while (true) {
                int splitIndex = name.IndexOf('.');
                string propertyName = splitIndex == -1 ? name : name.Substring(0, splitIndex);

                PropertyInfo property = properties.FirstOrDefault(p => p.Name == propertyName);
                if (property == null) return null;

                // Early exit
                if (splitIndex == -1) return property;

                // Resolve sub path
                string subPath = name.Substring(splitIndex + 1);
                PropertyInfo[] subProperties = property.PropertyType.GetTypeInfo().GetProperties();

                name = subPath;
                properties = subProperties;
            }
        }

        /// <summary>
        /// Converts the given string <paramref name="value"/> into the corresponding
        /// nullable value that is expected by the given <paramref name="property"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the property type is not nullable</exception>
        private object ToNullableValue(PropertyInfo property, string value) {
            if (string.IsNullOrEmpty(value)) return null;

            Type nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            if (nullableType == null) throw new InvalidOperationException("Cannot convert non-nullable to nullable");

            return ToTargetType(nullableType, value);
        }
        
        /// <summary>
        /// Converts the given string <paramref name="value"/> into the corresponding
        /// CLR enum value that is expected by the given <paramref name="property"/>.
        /// If no matching enum value is found, the default type-specific
        /// enumeration value is returned. 
        /// </summary>
        private object ToClrEnumValue(PropertyInfo property, string value) {
            Type enumType = property.PropertyType;
            string[] enumValueNames = Enum.GetNames(enumType);

            for (int i = -1; ++i != enumValueNames.Length;) {
                string enumValueName = enumValueNames[i];
                if (!string.Equals(enumValueName, value, StringComparison.InvariantCultureIgnoreCase)) continue;

                return Enum.GetValues(enumType).GetValue(i);
            }

            // Return default value
            return Activator.CreateInstance(enumType);
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> into the specified <paramref name="type"/>
        /// via <see cref="Convert.ChangeType(object,System.Type,IFormatProvider)"/>. <see cref="CultureInfo.InvariantCulture"/>
        /// is used as format provider.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object ToTargetType(Type type, object value) {
            return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Reads the settings defined by the referenced file.
        /// </summary>
        /// <param name="filePath">Path to the file that contains the configuration</param>
        /// <typeparam name="TObject">Type of object the values are assigned to</typeparam>
        /// <returns>Instance of <typeparamref name="TObject"/> the configation is assigned to</returns>
        public static TObject ReadFromFile<TObject>(string filePath) where TObject : new() {
            return new PlatesReader().ReadFile<TObject>(filePath);
        }
    }
}