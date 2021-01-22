namespace Faithlife.Utility.Tests
{
	public static class StringSegmentTestsExtensions
	{
		public static bool IsIdenticalTo(this StringSegment segA, StringSegment segB) => segA.Length == segB.Length && segA.Offset == segB.Offset && segA.Source == segB.Source;
	}
}
