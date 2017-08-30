using System;
using System.Collections.Generic;
using System.Reflection;

namespace Faithlife.Utility
{
	/// <summary>
	/// Provides helper methods for reflection operations.
	/// </summary>
	internal static class ReflectionUtility
	{
		/// <summary>
		/// Creates a collection of KeyValuePairs using the properties of the object
		/// </summary>
		/// <param name="values">The object to map from</param>
		/// <returns>A collection of KeyValuePairs where the keys are the property names and values are the property values.</returns>
		public static IEnumerable<KeyValuePair<string, object>> CreatePairsFromObject(object values)
		{
			foreach (PropertyInfo property in values.GetType().GetRuntimeProperties())
			{
				object value = property.GetValue(values);
				yield return new KeyValuePair<string, object>(property.Name, value);
			}
		}
	}
}
