namespace Faithlife.Utility
{
	/// <summary>
	/// Specifies how to handle key collisions in DictionaryUtility.MergeWith.
	/// </summary>
	public enum MergeWithStrategy
	{
		/// <summary>
		/// Retain the existing value for this key.
		/// </summary>
		KeepOriginalValue,

		/// <summary>
		/// Use the value from the passed-in dictionary instead of the current value.
		/// </summary>
		OverwriteValue,

		/// <summary>
		/// Throw an exception on any key conflict.
		/// </summary>
		ThrowException
	}
}
