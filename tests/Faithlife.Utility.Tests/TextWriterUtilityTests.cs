using System.IO;
using Faithlife.Utility.Threading;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class TextWriterUtilityTests
	{
		[Test]
		public void WriteBatched()
		{
			const string text = "abcd\uFF41\uFF42\uFF43\uFF44\uD800\uDD40\uD800\uDD41";
			using (StringWriter writer = new StringWriter())
			{
				writer.WriteBatched(text, 3, WorkState.None);
				writer.Flush();
				Assert.AreEqual(text, writer.ToString());
			}
		}
	}
}
