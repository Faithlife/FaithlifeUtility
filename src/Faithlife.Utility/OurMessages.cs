using System.CodeDom.Compiler;

#pragma warning disable 1591

namespace Faithlife.Utility
{
	[GeneratedCode("SuppressCodeAnalysis", "1.0")]
	internal static class OurMessages
	{
		public static readonly string Argument_ArrayIndexTooBig = "The number of elements in the source collection is greater than the available space in the destination array.";
		public static readonly string Argument_ArrayMultiDimensional = "Array must not be multi-dimensional.";
		public static readonly string Argument_CannotCastToArray = "The type of the source collection cannot be cast automatically to the type of the destination array.";
		public static readonly string Argument_CollectionReadOnly = "The collection must not be read-only.";
		public static readonly string Argument_CompareToInvalidObject = "obj is not a {0}";
		public static readonly string Argument_IndexPastLength = "Index is greater than or equal to the length of the array.";
		public static readonly string Argument_InvalidIndexCount = "The sum of index and count is larger than the size of the list.";
		public static readonly string Argument_InvalidOffsetCount = "The sum of offset and count is larger than the buffer length.";
		public static readonly string Argument_InvalidPath = "The path is invalid.";
		public static readonly string Argument_MissingAttributeValue = "The last attribute is missing its value.";
		public static readonly string Argument_NoAssemblyManifest = "The specified assembly does not contain a manifest.";
		public static readonly string Argument_NonZeroLowerBound = "The lower bound of the target array must be zero.";
		public static readonly string Argument_SegmentFromDifferentString = "The specified segment is from a different string.";
		public static readonly string Argument_WrongAsyncResult = "The asyncResult argument was not returned by the BeginExecute method on the current AsyncWorker.";
		public static readonly string ArgumentNull_Array = "Array cannot be null.";
		public static readonly string ArgumentNull_Buffer = "Buffer cannot be null.";
		public static readonly string ArgumentOutOfRange_BetweenZeroAndOne = "The value must be between 0 and 1.";
		public static readonly string ArgumentOutOfRange_MustBeNonNegative = "The parameter must be a non-negative number.";
		public static readonly string ArgumentOutOfRange_SmallCapacity = "Capacity cannot be less than the current size.";
		public static readonly string ArgumentOutOfRange_SortKeyIntegerMustNotBeMaxValue = "A sort key cannot be created for Int32.MaxValue.";
		public static readonly string AsyncWorkException_Message = "Unhandled exception in asynchronous work.";
		public static readonly string CreateCommandInvalidParameterName = "Parameter name at index {0} must be a non-empty string.";
		public static readonly string CreateCommandOddParameterCount = "Must supply even number of parameter names and values.";
		public static readonly string Cryptographic_IVMustBe8Bytes = "Invalid IV size; it must be 8 bytes.";
		public static readonly string Cryptographic_KeyMustBe128Or256Bits = "Invalid key size; it must be 128 or 256 bits.";
		public static readonly string InvalidBase64String = "Argument is not a valid base64 string.";
		public static readonly string InvalidCommandText = "The command text is invalid.";
		public static readonly string InvalidOperation_EmptyQueue = "The queue is empty.";
		public static readonly string InvalidOperation_NoOpenElementForAttributes = "WriteAttributes can only be called immediately after WriteStartElement.";
		public static readonly string InvalidOperation_ResultRequired = "The operation is required to yield a ResultAsyncAction.";
		public static readonly string IO_ReadBeyondEOF = "Unable to read beyond the end of the stream.";
		public static readonly string IO_SeekBeforeBegin = "An attempt was made to move the position before the beginning of the stream.";
		public static readonly string LanguageTagNotSupported = "The language tag '{0}' is not supported.";
		public static readonly string NoElements = "No elements were found.";
		public static readonly string NotSupported_StreamCannotRead = "Stream does not support reading.";
		public static readonly string NotSupported_StreamCannotWrite = "Stream does not support writing.";
		public static readonly string ObjectIsReadOnly = "Object is readonly. Clone() before changing properties.";
		public static readonly string ObjectOfTypeIsReadOnly = "The {0} is readonly. Clone() before changing properties.";
		public static readonly string StreamClosed = "Cannot access a closed stream.";
		public static readonly string XmlSubtreeReaderElementContentAsBinaryNotSupported = "XmlSubtreeReader cannot support ReadElementContentAsBase64/BinHex; use ReadContentAsBase64/BinHex instead.";
		public static readonly string XmlSubtreeReaderUnexpectedDepth = "Reader wrapped by XmlSubtreeReader was read beyond the end element of the subtree.";
	}
}
