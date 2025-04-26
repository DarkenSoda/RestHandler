using System;
using System.Collections.Generic;
using System.Reflection;

namespace DarkenSoda.Extensions
{
    /// <summary>
    /// Extension methods for object manipulation.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a dictionary where the keys are property names and the values are property values.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <returns>A dictionary representation of the object.</returns>
        public static Dictionary<string, string> ToDictionary(this object obj)
        {
            var dictionary = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj)?.ToString();
                if (value != null)
                {
                    dictionary.Add(property.Name, value);
                }
            }

            return dictionary;
        }
    }
}
