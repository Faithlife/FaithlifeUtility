using System.Collections.Generic;

namespace Faithlife.Utility.Tests
{
	internal class StringLengthComparer : IComparer<string>
	{
		public int Compare(string? x, string? y)
		{
			return x!.Length.CompareTo(y!.Length);
		}
	}
}
