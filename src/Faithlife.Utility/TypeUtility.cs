using System;
using System.Linq;
using System.Reflection;

namespace Faithlife.Utility
{
	/// <summary>
	/// Extension methods allowing portable and non portable libraries to call a single set of reflection based Type methods.
	/// </summary>
	public static class TypeUtility
	{
		/// <summary>
		/// Determines whether the type is abstract.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>true if the type is abstract.</returns>
		public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

		/// <summary>
		/// Determines whether an instance of the specified type can be assigned from an instance of the specified parent type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="parentType">The parent type.</param>
		/// <returns>true if parentType and type represent the same type, or if type is in the inheritance hierarchy of parent type, 
		/// or if type is an interface that parentType implements, or if parent type is a generic type parameter and type represents 
		/// one of the constraints of parent type. false if none of these conditions are true, or if parent type is null.</returns>
		public static bool IsAssignableFrom(this Type type, Type parentType) => type.GetTypeInfo().IsAssignableFrom(parentType.GetTypeInfo());

		/// <summary>
		/// Gets a value indicating whether the specified Type represents an enumeration.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>true if the current Type represents an enumeration; otherwise, false.</returns>
		public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

		/// <summary>
		/// Gets a value indicating whether the specified type is a generic type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>true if the specified type is a generic type; otherwise, false.</returns>
		public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

		/// <summary>
		/// Gets a value indicating whether the specified type is a class type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>true if the specified type is a class type; otherwise, false.</returns>
		public static bool IsClass(this Type type) => type.GetTypeInfo().IsClass;

		/// <summary>
		/// Determines whether an instance of the specified type is a subclass of the current type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="c">The type to compare.</param>
		/// <returns>true if the Type represented by the c parameter and the current Type represent classes, and the class
		/// represented by the current Type derives from the class represented by c; otherwise, false. This method also returns
		/// false if c and the current Type represent the same class.</returns>
		public static bool IsSubclassOf(this Type type, Type c) => type.GetTypeInfo().IsSubclassOf(c);

		/// <summary>
		/// Determines whether the specified Type is a value type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>true if specified type is a value type, false if not.</returns>
		public static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

		/// <summary>
		/// Gets the System.Reflection.Assembly in which the specified type is declared. For generic types, 
		/// gets the System.Reflection.Assembly in which the generic type is defined.
		/// </summary>
		/// <returns>
		/// An System.Reflection.Assembly instance that describes the assembly containing
		/// the specified type. For generic types, the instance describes the assembly
		/// that contains the generic type definition, not the assembly that creates
		/// and uses a particular constructed type.
		/// </returns>
		public static Assembly GetAssembly(this Type type) => type.GetTypeInfo().Assembly;

		/// <summary>
		/// Gets the type from which the specified Type directly inherits.
		/// </summary>
		public static Type GetBaseType(this Type type) => type.GetTypeInfo().BaseType;

		/// <summary>
		/// Gets the default constructor, or null if there isn't one.
		/// </summary>
		public static ConstructorInfo GetDefaultConstructor(this Type type) => type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => x.GetParameters().Length == 0);

		/// <summary>
		/// Gets the constructor with the specified parameter types, or null if there isn't one.
		/// </summary>
		public static ConstructorInfo GetConstructor(this Type type, Type[] types)
			=> type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => x.IsPublic && EnumerableUtility.AreEqual(x.GetParameters().Select(p => p.ParameterType), types));

		/// <summary>
		/// Gets an array of the generic type arguments for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>An array of the generic type arguments for the specified type.</returns>
		public static Type[] GetGenericArguments(this Type type) => type.GenericTypeArguments;

		/// <summary>
		/// Gets the properties.
		/// </summary>
		public static PropertyInfo[] GetProperties(this Type type) => type.GetRuntimeProperties().Where(x => x.GetMethod is object).ToArray();

		/// <summary>
		/// Gets the property.
		/// </summary>
		public static PropertyInfo GetProperty(this Type type, string propertyName, bool ignoreCase = false)
			=> type.GetRuntimeProperties().FirstOrDefault(x => x.GetMethod is object && string.Equals(x.Name, propertyName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		public static object[] GetCustomAttributes(this Type type, bool inherit) => type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		public static object[] GetCustomAttributes(this Type type, Type attributeType, bool inherit) => type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
	}
}
