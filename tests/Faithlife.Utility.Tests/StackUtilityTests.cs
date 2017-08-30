using System.Collections.Generic;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StackUtilityTests
	{
		[Test]
		public void PeekOrDefault()
		{
			Stack<int> stack = new Stack<int>();
			Assert.AreEqual(0, StackUtility.PeekOrDefault(stack));
			stack.Push(3);
			Assert.AreEqual(3, StackUtility.PeekOrDefault(stack));
			stack.Pop();
			Assert.AreEqual(0, StackUtility.PeekOrDefault(stack));
		}

		[Test]
		public void PeekOrDefaultWithDefault()
		{
			Stack<int> stack = new Stack<int>();
			Assert.AreEqual(1, StackUtility.PeekOrDefault(stack, 1));
			stack.Push(3);
			Assert.AreEqual(3, StackUtility.PeekOrDefault(stack, 1));
			stack.Pop();
			Assert.AreEqual(1, StackUtility.PeekOrDefault(stack, 1));
		}
	}
}
