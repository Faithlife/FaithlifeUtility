namespace Faithlife.Utility
{
	/// <summary>
	/// Provides methods for manipulating integers.
	/// </summary>
	public static class HashCodeUtility
	{
		/// <summary>
		/// Gets a hash code for the specified <see cref="int"/>; this hash code is guaranteed not to change in future.
		/// </summary>
		/// <param name="value">The <see cref="int"/> to hash.</param>
		/// <returns>A hash code for the specified <see cref="int"/>.</returns>
		/// <remarks>Based on "Robert Jenkins' 32 bit integer hash function" at http://www.concentric.net/~Ttwang/tech/inthash.htm</remarks>
		public static int GetPersistentHashCode(int value)
		{
			unchecked
			{
				var n = (uint) value;
				n = (n + 0x7ed55d16) + (n << 12);
				n = (n ^ 0xc761c23c) ^ (n >> 19);
				n = (n + 0x165667b1) + (n << 5);
				n = (n + 0xd3a2646c) ^ (n << 9);
				n = (n + 0xfd7046c5) + (n << 3);
				n = (n ^ 0xb55a4f09) ^ (n >> 16);
				return (int) n;
			}
		}

		/// <summary>
		/// Gets a hash code for the specified <see cref="long"/>; this hash code is guaranteed not to change in future.
		/// </summary>
		/// <param name="value">The <see cref="long"/> to hash.</param>
		/// <returns>A hash code for the specified <see cref="long"/>.</returns>
		/// <remarks>Based on "64 bit to 32 bit Hash Functions" at http://www.concentric.net/~Ttwang/tech/inthash.htm</remarks>
		public static int GetPersistentHashCode(long value)
		{
			unchecked
			{
				var n = (ulong) value;
				n = (~n) + (n << 18);
				n = n ^ (n >> 31);
				n = n * 21;
				n = n ^ (n >> 11);
				n = n + (n << 6);
				n = n ^ (n >> 22);
				return (int) n;
			}
		}

		/// <summary>
		/// Gets a hash code for the specified <see cref="bool"/>; this hash code is guaranteed not to change in future.
		/// </summary>
		/// <param name="value">The <see cref="bool"/> to hash.</param>
		/// <returns>A hash code for the specified <see cref="bool"/>.</returns>
		public static int GetPersistentHashCode(bool value)
		{
			// these values are the persistent hash codes for 0 and 1
			return value ? -1266253386 : 1800329511;
		}

		/// <summary>
		/// Combines the specified hash codes.
		/// </summary>
		/// <param name="hashCode1">The first hash code.</param>
		/// <returns>The combined hash code.</returns>
		/// <remarks>This is a specialization of <see cref="CombineHashCodes(int[])"/> for efficiency.</remarks>
		public static int CombineHashCodes(int hashCode1)
		{
			unchecked
			{
				var a = 0xdeadbeef + 4;
				var b = a;
				var c = a;

				a += (uint) hashCode1;
				FinalizeHash(ref a, ref b, ref c);

				return (int) c;
			}
		}

		/// <summary>
		/// Combines the specified hash codes.
		/// </summary>
		/// <param name="hashCode1">The first hash code.</param>
		/// <param name="hashCode2">The second hash code.</param>
		/// <returns>The combined hash code.</returns>
		/// <remarks>This is a specialization of <see cref="CombineHashCodes(int[])"/> for efficiency.</remarks>
		public static int CombineHashCodes(int hashCode1, int hashCode2)
		{
			unchecked
			{
				var a = 0xdeadbeef + 8;
				var b = a;
				var c = a;

				a += (uint) hashCode1;
				b += (uint) hashCode2;
				FinalizeHash(ref a, ref b, ref c);

				return (int) c;
			}
		}

		/// <summary>
		/// Combines the specified hash codes.
		/// </summary>
		/// <param name="hashCode1">The first hash code.</param>
		/// <param name="hashCode2">The second hash code.</param>
		/// <param name="hashCode3">The third hash code.</param>
		/// <returns>The combined hash code.</returns>
		/// <remarks>This is a specialization of <see cref="CombineHashCodes(int[])"/> for efficiency.</remarks>
		public static int CombineHashCodes(int hashCode1, int hashCode2, int hashCode3)
		{
			unchecked
			{
				var a = 0xdeadbeef + 12;
				var b = a;
				var c = a;

				a += (uint) hashCode1;
				b += (uint) hashCode2;
				c += (uint) hashCode3;
				FinalizeHash(ref a, ref b, ref c);

				return (int) c;
			}
		}

		/// <summary>
		/// Combines the specified hash codes.
		/// </summary>
		/// <param name="hashCode1">The first hash code.</param>
		/// <param name="hashCode2">The second hash code.</param>
		/// <param name="hashCode3">The third hash code.</param>
		/// <param name="hashCode4">The fourth hash code.</param>
		/// <returns>The combined hash code.</returns>
		/// <remarks>This is a specialization of <see cref="CombineHashCodes(int[])"/> for efficiency.</remarks>
		public static int CombineHashCodes(int hashCode1, int hashCode2, int hashCode3, int hashCode4)
		{
			unchecked
			{
				var a = 0xdeadbeef + 16;
				var b = a;
				var c = a;

				a += (uint) hashCode1;
				b += (uint) hashCode2;
				c += (uint) hashCode3;
				MixHash(ref a, ref b, ref c);

				a += (uint) hashCode4;
				FinalizeHash(ref a, ref b, ref c);

				return (int) c;
			}
		}

		/// <summary>
		/// Combines the specified hash codes.
		/// </summary>
		/// <param name="hashCodes">An array of hash codes.</param>
		/// <returns>The combined hash code.</returns>
		/// <remarks>This method is based on the "hashword" function at http://burtleburtle.net/bob/c/lookup3.c. It attempts to thoroughly
		/// mix all the bits in the input hash codes.</remarks>
		public static int CombineHashCodes(params int[]? hashCodes)
		{
			unchecked
			{
				// check for null
				if (hashCodes is null)
					return 0x0d608219;

				var nLength = hashCodes.Length;

				var a = 0xdeadbeef + (((uint) nLength) << 2);
				var b = a;
				var c = a;

				var nIndex = 0;
				while (nLength - nIndex > 3)
				{
					a += (uint) hashCodes[nIndex];
					b += (uint) hashCodes[nIndex + 1];
					c += (uint) hashCodes[nIndex + 2];
					MixHash(ref a, ref b, ref c);
					nIndex += 3;
				}

				if (nLength - nIndex > 2)
					c += (uint) hashCodes[nIndex + 2];
				if (nLength - nIndex > 1)
					b += (uint) hashCodes[nIndex + 1];

				if (nLength - nIndex > 0)
				{
					a += (uint) hashCodes[nIndex];
					FinalizeHash(ref a, ref b, ref c);
				}

				return (int) c;
			}
		}

		// The "rot()" macro from http://burtleburtle.net/bob/c/lookup3.c
		private static uint Rotate(uint x, int k)
		{
			return (x << k) | (x >> (32 - k));
		}

		// The "mix()" macro from http://burtleburtle.net/bob/c/lookup3.c
		private static void MixHash(ref uint a, ref uint b, ref uint c)
		{
			unchecked
			{
				a -= c;
				a ^= Rotate(c, 4);
				c += b;
				b -= a;
				b ^= Rotate(a, 6);
				a += c;
				c -= b;
				c ^= Rotate(b, 8);
				b += a;
				a -= c;
				a ^= Rotate(c, 16);
				c += b;
				b -= a;
				b ^= Rotate(a, 19);
				a += c;
				c -= b;
				c ^= Rotate(b, 4);
				b += a;
			}
		}

		// The "final()" macro from http://burtleburtle.net/bob/c/lookup3.c
		private static void FinalizeHash(ref uint a, ref uint b, ref uint c)
		{
			unchecked
			{
				c ^= b;
				c -= Rotate(b, 14);
				a ^= c;
				a -= Rotate(c, 11);
				b ^= a;
				b -= Rotate(a, 25);
				c ^= b;
				c -= Rotate(b, 16);
				a ^= c;
				a -= Rotate(c, 4);
				b ^= a;
				b -= Rotate(a, 14);
				c ^= b;
				c -= Rotate(b, 24);
			}
		}
	}
}
