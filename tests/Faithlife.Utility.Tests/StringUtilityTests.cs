using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Faithlife.Utility.Tests
{
	[TestFixture]
	public class StringUtilityTests
	{
		[TestCase("", 'a', false)]
		[TestCase("", '\u0000', false)]
		[TestCase("a", 'a', true)]
		[TestCase("ab", 'a', false)]
		[TestCase("aaaaaaaaaab", 'a', false)]
		[TestCase("aaaaaaaaaab", 'b', true)]
		public void EndsWith(string str, char ch, bool expected)
		{
			Assert.AreEqual(expected, str.EndsWith(ch));
		}

		[Test]
		public void OrdinalStringComparisonExtensionMethods()
		{
			// demonstrate the problem; this is more expensive and possibly surprising
			Assert.IsTrue("ábá".StartsWith("a\u0301"));
			Assert.IsTrue("ábá".EndsWith("a\u0301"));
			Assert.AreEqual(0, "ábá".IndexOf("a\u0301"));
			Assert.AreEqual(2, "ábá".LastIndexOf("a\u0301"));

			// show how using Ordinal fixes the problem
			Assert.IsFalse("ábá".StartsWithOrdinal("a\u0301"));
			Assert.IsFalse("ábá".EndsWithOrdinal("a\u0301"));
			Assert.AreEqual(-1, "ábá".IndexOfOrdinal("a\u0301"));
			Assert.AreEqual(-1, "ábá".LastIndexOfOrdinal("a\u0301"));

			// confirm that we didn't break anything
			Assert.IsTrue("abc".StartsWithOrdinal("ab"));
			Assert.IsFalse("abc".StartsWithOrdinal("bc"));
			Assert.IsFalse("abc".EndsWithOrdinal("ab"));
			Assert.IsTrue("abc".EndsWithOrdinal("bc"));
			Assert.AreEqual(1, "abcabc".IndexOfOrdinal("bc"));
			Assert.AreEqual(-1, "abcabc".IndexOfOrdinal("ba"));
			Assert.AreEqual(4, "abcabc".IndexOfOrdinal("bc", 2));
			Assert.AreEqual(-1, "abcabc".IndexOfOrdinal("bc", 5));
			Assert.AreEqual(4, "abcabc".IndexOfOrdinal("bc", 2, 4));
			Assert.AreEqual(-1, "abcabc".IndexOfOrdinal("bc", 2, 3));
			Assert.AreEqual(4, "abcabc".LastIndexOfOrdinal("bc"));
			Assert.AreEqual(-1, "abcabc".LastIndexOfOrdinal("ba"));
			Assert.AreEqual(1, "abcabc".LastIndexOfOrdinal("bc", 4));
			Assert.AreEqual(-1, "abcabc".LastIndexOfOrdinal("bc", 0));
			Assert.AreEqual(1, "abcabc".LastIndexOfOrdinal("bc", 4, 4));
			Assert.AreEqual(-1, "abcabc".LastIndexOfOrdinal("bc", 4, 3));
		}

		[Test]
		public void FormatInvariant()
		{
			DateTime dtNow = DateTime.Now;
			Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0}!", dtNow),
				StringUtility.FormatInvariant("{0}!", dtNow));
		}

		[Test]
		public void JoinStrings()
		{
			LinkedList<string> linked = new LinkedList<string>();
			linked.AddLast("one");
			linked.AddLast(default(string));
			linked.AddLast("two");
			linked.AddLast("");
			linked.AddLast("three");
			Assert.AreEqual("one, , two, , three", linked.Join(", "));
			Assert.AreEqual("one,,two,,three", linked.Join(","));
			Assert.AreEqual("onetwothree", linked.Join());
			Assert.AreEqual("onetwothree", linked.Join(""));
			Assert.AreEqual("onetwothree", linked.Join(null));
		}

		[TestCase(0, new string[0])]
		[TestCase(1, new string[] { null })]
		[TestCase(2, new[] { null, "one", null, "", "three" })]
		[TestCase(3, new[] { "one" })]
		[TestCase(4, new[] { "one", "two", "three" })]
		public void JoinStringsInEnumerable(int nIgnored, string[] astrStrings)
		{
			const string strSeparator = ", ";
			string strActual = ((IEnumerable<string>) astrStrings).Join(strSeparator);
			Assert.AreEqual(string.Join(strSeparator, astrStrings), strActual);
			strActual = ((IEnumerable<string>) astrStrings).Join();
			Assert.AreEqual(string.Join(string.Empty, astrStrings), strActual);
			strActual = ((IEnumerable<string>) astrStrings).Join(string.Empty);
			Assert.AreEqual(string.Join(string.Empty, astrStrings), strActual);
			strActual = ((IEnumerable<string>) astrStrings).Join(null);
			Assert.AreEqual(string.Join(null, astrStrings), strActual);
		}

		[Test]
		public void JoinStringsNull()
		{
			string[] nullStrings = null;
			Assert.Throws<ArgumentNullException>(() => nullStrings.Join(", "));
		}

		[Test]
		public void JoinFormat()
		{
			string[] strings = new[] { "one", "two", "three" };
			Assert.AreEqual("one, two, three", strings.JoinFormat("{0}, {1}"));
			Assert.AreEqual("three, two, one", strings.JoinFormat("{1}, {0}"));
			Assert.AreEqual("((one, two), three)", strings.JoinFormat("({0}, {1})"));
		}

		[Test]
		public void JoinIntegers()
		{
			LinkedList<int> linked = new LinkedList<int>();
			linked.AddLast(1);
			linked.AddLast(2);
			linked.AddLast(3);
			Assert.AreEqual("1, 2, 3", linked.Select(x => x.ToString()).Join(", "));
		}

		[Test]
		public void JoinSquares()
		{
			LinkedList<int> linked = new LinkedList<int>();
			linked.AddLast(1);
			linked.AddLast(2);
			linked.AddLast(3);
			Assert.AreEqual("1, 4, 9", linked.Select(x => x * x).Select(x => x.ToString()).Join(", "));
		}

		[Test]
		public void JoinStringArray()
		{
			Assert.AreEqual("onetwothree", new[] { "one", "two", "three" }.Join());
			Assert.AreEqual("one, two, three", new[] { "one", "two", "three" }.Join(", "));
		}

		[Test]
		public void ReverseNull()
		{
			Assert.Throws<ArgumentNullException>(() => StringUtility.Reverse(null));
		}

		[TestCase("", "")]
		[TestCase("a", "a")]
		[TestCase("one", "eno")]
		[TestCase("Madam, I'm Adam", "madA m'I ,madaM")]
		[TestCase("A man, a plan, a cat, a ham, a yak, a yam, a hat, a canal – Panama!", "!amanaP – lanac a ,tah a ,may a ,kay a ,mah a ,tac a ,nalp a ,nam A")]
		public void Reverse(string strInput, string strReversed)
		{
			Assert.AreEqual(strReversed, StringUtility.Reverse(strInput));
			Assert.AreEqual(strInput, StringUtility.Reverse(strReversed));
			Assert.AreEqual(strInput, StringUtility.Reverse(StringUtility.Reverse(strInput)));
		}

		[Test]
		public void ReverseNonBmp()
		{
			int[] anCodePoints = new int[] { 0x10380, 0x10381, 0x10382, 0x1039F };

			StringBuilder sbForward = new StringBuilder();
			StringBuilder sbReversed = new StringBuilder();
			for (int nIndex = 0; nIndex < anCodePoints.Length; ++nIndex)
			{
				sbForward.Append(char.ConvertFromUtf32(anCodePoints[nIndex]));
				sbReversed.Append(char.ConvertFromUtf32(anCodePoints[anCodePoints.Length - nIndex - 1]));
			}

			Assert.AreEqual(sbReversed.ToString(), StringUtility.Reverse(sbForward.ToString()));
		}

		// TODO: is this a valid test?
		//#if !__MOBILE__
		//		// The Mono C# compiler has a fit with these strings for reasons
		//		// that have yet to be investigated.
		//		[TestCase("\uD800", "\uD800")]
		//		[TestCase("Z\uD800", "\uD800Z")]
		//		[TestCase("Z\uD800\uD801", "\uD801\uD800Z")]
		//		[TestCase("Z\uD800\uDC00\uDC01", "\uDC01\uD800\uDC00Z")]
		//		public void ReverseMalformed(string strInput, string strReversed)
		//		{
		//			Assert.AreEqual(strReversed, StringUtility.Reverse(strInput));
		//			Assert.AreEqual(strInput, StringUtility.Reverse(strReversed));
		//			Assert.AreEqual(strInput, StringUtility.Reverse(StringUtility.Reverse(strInput)));
		//		}
		//#endif

		[TestCase(null, "", -1)]
		[TestCase(null, null, 0)]
		[TestCase("", "", 0)]
		[TestCase("abc", "abcd", -1)]
		[TestCase("abd", "abcd", 1)]
		[TestCase("Abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz", -1)]
		[TestCase("abcdefghijklmnopqrstuvwxyZ", "abcdefghijklmnopqrstuvwxyz", -1)]
		[TestCase("abcdefghijklmnopqrstuvwxyz", "abcdefghijklmnopqrstuvwxyz", 0)]
		[TestCase("\uD700", "\uE000", -1)]
		[TestCase("\uD700", "\uD800\uDF80", -1)]
		[TestCase("\uE001", "\uD800\uDF80", -1)]
		public void CompareByCodePoint(string strOne, string strTwo, int nExpectedResult)
		{
			int nActual12 = StringUtility.CompareByCodePoint(strOne, strTwo);
			int nActual21 = StringUtility.CompareByCodePoint(strTwo, strOne);

			if (nExpectedResult < 0)
			{
				Assert.Less(nActual12, 0);
				Assert.Greater(nActual21, 0);
			}
			else if (nExpectedResult == 0)
			{
				Assert.AreEqual(0, nActual12);
				Assert.AreEqual(0, nActual21);
			}
			else
			{
				Assert.Greater(nActual12, 0);
				Assert.Less(nActual21, 0);
			}
		}

		// Latin
		[TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz")]
		[TestCase("ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏ", "àáâãäåæçèéêëìíîï")]
		[TestCase("ÐÑÒÓÔÕÖØÙÚÛÜÝÞ", "ðñòóôõöøùúûüýþ")]
		[TestCase("SSßss", "ssssss")]
		[TestCase("µ", "μ")]
		[TestCase("ſ", "s")]
		// Latin ligatures
		[TestCase("ﬀﬁﬂﬃﬄﬅﬆ", "fffiflffifflstst")]
		[TestCase("Ǆǅǆ", "ǆǆǆ")]
		[TestCase("Ǳǲǳ", "ǳǳǳ")]
		[TestCase("Ǉǈǉ", "ǉǉǉ")]
		[TestCase("Ǌǋǌ", "ǌǌǌ")]
		[TestCase("J̌ǰǰ", "ǰǰǰ")]
		// extended Latin and IPA
		[TestCase("ĀĂĄĆĈĊČĎĐĒĔĖĘĚĜĞĠĢĤĦĨĪĬĮİĲĴĶĹĻĽĿŁ", "āăąćĉċčďđēĕėęěĝğġģĥħĩīĭįi̇ĳĵķĺļľŀł")]
		[TestCase("ŃŅŇŊŌŎŐŒŔŖŘŚŜŞŠŢŤŦŨŪŬŮŰŲŴŶŸŹŻŽ", "ńņňŋōŏőœŕŗřśŝşšţťŧũūŭůűųŵŷÿźżž")]
		[TestCase("ŉ", "ʼn")]
		[TestCase("ʼN", "ʼn")]
		[TestCase("ɃƁƂƄƆƇƉƊƋƎƏƐƑƓƔǶƖƗƘȽƜƝȠƟƠƢƤƦƧƩƬƮƯƱƲƳƵƷƸƼǷ", "ƀɓƃƅɔƈɖɗƌǝəɛƒɠɣƕɩɨƙƚɯɲƞɵơƣƥʀƨʃƭʈưʊʋƴƶʒƹƽƿ")]
		[TestCase("ǍǏǑǓǕǗǙǛǞǠǢǤǦǨǪǬǮǴǶǸǺǼǾ", "ǎǐǒǔǖǘǚǜǟǡǣǥǧǩǫǭǯǵƕǹǻǽǿ")]
		[TestCase("ȀȂȄȆȈȊȌȎȐȒȔȖȘȚȜȞȢȤ", "ȁȃȅȇȉȋȍȏȑȓȕȗșțȝȟȣȥ")]
		[TestCase("ȦȨȪȬȮȰȲȺȻȾɁɄɅɆɈɊɌɎ", "ȧȩȫȭȯȱȳⱥȼⱦɂʉʌɇɉɋɍɏ")]
		[TestCase("ḀḂḄḆḈḊḌḎḐḒḔḖḘḚḜḞḠḢḤḦḨḪḬḮḰḲḴḶḸḺḼḾ", "ḁḃḅḇḉḋḍḏḑḓḕḗḙḛḝḟḡḣḥḧḩḫḭḯḱḳḵḷḹḻḽḿ")]
		[TestCase("ṀṂṄṆṈṊṌṎṐṒṔṖṘṚṜṞṠṢṤṦṨṪṬṮṰṲṴṶṸṺṼṾ", "ṁṃṅṇṉṋṍṏṑṓṕṗṙṛṝṟṡṣṥṧṩṫṭṯṱṳṵṷṹṻṽṿ")]
		[TestCase("ẀẂẄẆẈẊẌẎẐẒẔ", "ẁẃẅẇẉẋẍẏẑẓẕ")]
		[TestCase("ẖẗẘẙẚẛ", "ẖẗẘẙaʾṡ")]
		[TestCase("ẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼẾỀỂỄỆỈỊỌỎỐỒỔỖỘỚỜỞ", "ạảấầẩẫậắằẳẵặẹẻẽếềểễệỉịọỏốồổỗộớờở")]
		[TestCase("ỠỢỤỦỨỪỬỮỰỲỴỶỸ", "ỡợụủứừửữựỳỵỷỹ")]
		[TestCase("ΩKÅℲ", "ωkåⅎ")]
		[TestCase("ⱠⱢⱣⱤⱧⱩⱫⱵ", "ⱡɫᵽɽⱨⱪⱬⱶ")]
		[TestCase("ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯↃ", "ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿↄ")]
		[TestCase("ⒶⒷⒸⒹⒺⒻⒼⒽⒾⒿⓀⓁⓂⓃⓄⓅⓆⓇⓈⓉⓊⓋⓌⓍⓎⓏ", "ⓐⓑⓒⓓⓔⓕⓖⓗⓘⓙⓚⓛⓜⓝⓞⓟⓠⓡⓢⓣⓤⓥⓦⓧⓨⓩ")]
		[TestCase("ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ", "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ")]
		// Greek
		[TestCase("ᾼᾼΑιᾳᾳαι", "αιαιαιαιαιαι")]
		[TestCase("ΆΈΉΊΌΎΏΪΫ", "άέήίόύώϊϋ")]
		[TestCase("ΐΰΐΰ", "ΐΰΐΰ")]
		[TestCase("ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ", "αβγδεζηθικλμνξοπρστυφχψω")]
		[TestCase("Σςσ", "σσσ")]
		[TestCase("ἈἉἊἋἌἍἎἏ", "ἀἁἂἃἄἅἆἇ")]
		[TestCase("ἘἙἚἛἜἝ", "ἐἑἒἓἔἕ")]
		[TestCase("ἨἩἪἫἬἭἮἯ", "ἠἡἢἣἤἥἦἧ")]
		[TestCase("ἸἹἺἻἼἽἾἿ", "ἰἱἲἳἴἵἶἷ")]
		[TestCase("ὈὉὊὋὌὍ", "ὀὁὂὃὄὅ")]
		[TestCase("ὙὛὝὟ", "ὑὓὕὗ")]
		[TestCase("ὐὒὔὖ", "ὐὒὔὖ")]
		[TestCase("ὨὩὪὫὬὭὮὯ", "ὠὡὢὣὤὥὦὧ")]
		[TestCase("ᾀᾁᾂᾃᾄᾅᾆᾇᾈᾉᾊᾋᾌᾍᾎᾏ", "ἀιἁιἂιἃιἄιἅιἆιἇιἀιἁιἂιἃιἄιἅιἆιἇι")]
		[TestCase("ᾐᾑᾒᾓᾔᾕᾖᾗᾘᾙᾚᾛᾜᾝᾞᾟ", "ἠιἡιἢιἣιἤιἥιἦιἧιἠιἡιἢιἣιἤιἥιἦιἧι")]
		[TestCase("ᾠᾡᾢᾣᾤᾥᾦᾧᾨᾩᾪᾫᾬᾭᾮᾯ", "ὠιὡιὢιὣιὤιὥιὦιὧιὠιὡιὢιὣιὤιὥιὦιὧι")]
		[TestCase("ϐϑϕϖ", "βθφπ")]
		[TestCase("ϘϚϜϞϠ", "ϙϛϝϟϡ")]
		[TestCase("ϰϱϹϴϵϷϺϽϾϿ", "κρϲθεϸϻͻͼͽ")]
		[TestCase("ᾲᾳᾴᾶᾷᾸᾹᾺΆᾼ", "ὰιαιάιᾶᾶιᾰᾱὰάαι")]
		[TestCase("ι", "ι")]
		[TestCase("ῂῃῄῆῇῈΈῊΉῌ", "ὴιηιήιῆῆιὲέὴήηι")]
		[TestCase("ῒΐῖῗῘῙῚΊ", "ῒΐῖῗῐῑὶί")]
		[TestCase("ῢΰῤῦῧῨῩῪΎῬ", "ῢΰῤῦῧῠῡὺύῥ")]
		[TestCase("ῲῳῴῶῷῸΌῺΏῼ", "ὼιωιώιῶῶιὸόὼώωι")]
		// Coptic
		[TestCase("ϢϤϦϨϪϬϮ", "ϣϥϧϩϫϭϯ")]
		[TestCase("ⲀⲂⲄⲆⲈⲊⲌⲎⲐⲒⲔⲖⲘⲚⲜⲞⲠⲢⲤⲦⲨⲪⲬⲮⲰⲲⲴⲶⲸⲺⲼⲾⳀⳂⳄⳆⳈⳊⳌⳎⳐⳒⳔⳖⳘⳚⳜⳞⳠⳢ", "ⲁⲃⲅⲇⲉⲋⲍⲏⲑⲓⲕⲗⲙⲛⲝⲟⲡⲣⲥⲧⲩⲫⲭⲯⲱⲳⲵⲷⲹⲻⲽⲿⳁⳃⳅⳇⳉⳋⳍⳏⳑⳓⳕⳗⳙⳛⳝⳟⳡⳣ")]
		// Cyrillic
		[TestCase("АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ", "абвгдежзийклмнопрстуфхцчшщъыьэюя")]
		[TestCase("ЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏ", "ѐёђѓєѕіїјљњћќѝўџ")]
		[TestCase("ѠѢѤѦѨѪѬѮѰѲѴѶѸѺѼѾҀ", "ѡѣѥѧѩѫѭѯѱѳѵѷѹѻѽѿҁ")]
		[TestCase("ҊҌҎҐҒҔҖҘҚҜҞҠҢҤҦҨҪҬҮҰҲҴҶҸҺҼҾ", "ҋҍҏґғҕҗҙқҝҟҡңҥҧҩҫҭүұҳҵҷҹһҽҿ")]
		[TestCase("ӀӁӃӅӇӉӋӍӐӒӔӖӘӚӜӞӠӢӤӦӨӪӬӮӰӲӴӶӸӺӼӾԀԂԄԆԈԊԌԎԐԒ", "ӏӂӄӆӈӊӌӎӑӓӕӗәӛӝӟӡӣӥӧөӫӭӯӱӳӵӷӹӻӽӿԁԃԅԇԉԋԍԏԑԓ")]
		// Armenian
		[TestCase("ԱԲԳԴԵԶԷԸԹԺԻԼԽԾԿՀՁՂՃՄՅՆՇՈՉՊՋՌՍՎՏՐՑՒՓՔՕՖ", "աբգդեզէըթժիլխծկհձղճմյնշոչպջռսվտրցւփքօֆ")]
		[TestCase("և", "եւ")]
		[TestCase("ﬓﬔﬕﬖﬗ", "մնմեմիվնմխ")]
		// Georgian
		[TestCase("ႠႡႢႣႤႥႦႧႨႩႪႫႬႭႮႯႰႱႲႳႴႵႶႷႸႹႺႻႼႽႾႿჀჁჂჃჄჅ", "ⴀⴁⴂⴃⴄⴅⴆⴇⴈⴉⴊⴋⴌⴍⴎⴏⴐⴑⴒⴓⴔⴕⴖⴗⴘⴙⴚⴛⴜⴝⴞⴟⴠⴡⴢⴣⴤⴥ")]
		// Glagolitic
		[TestCase("ⰀⰁⰂⰃⰄⰅⰆⰇⰈⰉⰊⰋⰌⰍⰎⰏⰐⰑⰒⰓⰔⰕⰖⰗⰘⰙⰚⰛⰜⰝⰞⰟⰠⰡⰢⰣⰤⰥⰦⰧⰨⰩⰪⰫⰬⰭⰮ", "ⰰⰱⰲⰳⰴⰵⰶⰷⰸⰹⰺⰻⰼⰽⰾⰿⱀⱁⱂⱃⱄⱅⱆⱇⱈⱉⱊⱋⱌⱍⱎⱏⱐⱑⱒⱓⱔⱕⱖⱗⱘⱙⱚⱛⱜⱝⱞ")]
		// Hebrew (uncased)
		[TestCase("אבגדהוזחטיךכלםמןנסעףפץצקרשת", "אבגדהוזחטיךכלםמןנסעףפץצקרשת")]
		// Syriac (uncased)
		[TestCase("ܐܑܒܓܔܕܖܗܘܙܚܛܜܝܞܟܠܡܢܣܤܥܦܧܨܩܪܫܬ", "ܐܑܒܓܔܕܖܗܘܙܚܛܜܝܞܟܠܡܢܣܤܥܦܧܨܩܪܫܬ")]
		public void FoldCase(string strInput, string strExpected)
		{
			// test input
			Assert.AreEqual(strExpected, StringUtility.FoldCase(strInput));

			// test idempotence
			Assert.AreEqual(strExpected, StringUtility.FoldCase(strExpected));
		}

		[Test]
		public void FoldCaseNull()
		{
			Assert.Throws<ArgumentNullException>(() => StringUtility.FoldCase(null));
		}

		[TestCase("", 0)]
		[TestCase(null, 0)]
		[TestCase("a", -889528276)]
		[TestCase("b", -685344420)]
		[TestCase("c", -414938692)]
		[TestCase("abc", 2058321224)]
		[TestCase("abcabc", 120553164)]
		[TestCase("abcabca", 451022788)]
		[TestCase("\" \"", 1671599841)]
		[TestCase("#@!", 1671599841)]
		public void GetPersistentHash(string s, int nExpected)
		{
			Assert.AreEqual(nExpected, StringUtility.GetPersistentHashCode(s));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("1")]
		[TestCase("ⒶⒷⒸⒹⒺⒻⒼⒽⒾⒿⓀⓁⓂⓃⓄⓅⓆⓇⓈⓉⓊⓋⓌⓍⓎⓏ")]
		public void CompressAndDecompress(string text)
		{
			byte[] bytes = StringUtility.Compress(text);
			Assert.AreEqual(StringUtility.Decompress(bytes), text);
		}

		[Test]
		public void CompressAndDecompressShortString()
		{
			const string text = "1234";
			byte[] bytes = StringUtility.Compress(text);
			Assert.AreEqual(bytes[0], (byte) 2);
			Assert.AreEqual(StringUtility.Decompress(bytes), text);
		}

		[Test]
		public void CompressAndDecompressLongString()
		{
			StringBuilder textBuilder = new StringBuilder();
			for (int i = 0; i < 100000; i++)
				textBuilder.AppendLine(Guid.NewGuid().ToString());
			string text = textBuilder.ToString();
			byte[] bytes = StringUtility.Compress(text);
			Assert.AreEqual(bytes[0], (byte) 1);
			Assert.AreEqual(StringUtility.Decompress(bytes), text);
		}

		[TestCase(null, null)]
		[TestCase("01-01-00-00-00-1F-8B-08-00-00-00-00-00-04-00-ED-BD-07-60-1C-49-96-25-26-2F-6D-CA-7B-7F-4A-F5-4A-D7-E0-74-A1-08-80-60-13-24-D8-90-40-10-EC-C1-88-CD-E6-92-EC-1D-69-47-23-29-AB-2A-81-CA-65-56-65-5D-66-16-40-CC-ED-9D-BC-F7-DE-7B-EF-BD-F7-DE-7B-EF-BD-F7-BA-3B-9D-4E-27-F7-DF-FF-3F-5C-66-64-01-6C-F6-CE-4A-DA-C9-9E-21-80-AA-C8-1F-3F-7E-7C-1F-3F-22-76-FF-1F-B7-EF-DC-83-01-00-00-00", "1")]
		[TestCase("02-01-00-00-00-31", "1")]
		[TestCase("01-00-00-00-00", "")]
		[TestCase("02-00-00-00-00", "")]
		public void Decompress(string data, string text)
		{
			byte[] bytes = data == null ? null :
				data.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
			Assert.AreEqual(StringUtility.Decompress(bytes), text);
		}

		[TestCase("00-00-00-00-00", null)]
		[TestCase("03-00-00-00-00", null)]
		[TestCase("01-00-00", null)]
		[TestCase("01-00-00-00-01-00", "")]
		public void DecompressFormatException(string data, string text)
		{
			byte[] bytes = data == null ? null :
				data.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
			Assert.Throws<FormatException>(() => StringUtility.Decompress(bytes));
		}

		[Test]
		public void CompressionTextWriterAndTextReader()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				using (TextWriter textWriter = StringUtility.CreateCompressingTextWriter(stream, Ownership.None))
				{
					textWriter.Write('a');
					textWriter.Write(new[] { 'b', 'c', 'd' });
					textWriter.Write(new[] { 'd', 'e', 'f', 'g', 'h' }, 1, 3);
					textWriter.WriteLine("line");
					textWriter.WriteLine(1234);
				}

				stream.Flush();
				stream.Position = 0;

				using (TextReader textReader = StringUtility.CreateDecompressingTextReader(stream, Ownership.None))
				{
					char[] buffer = new char[5];
					textReader.ReadBlock(buffer, 1, 3);
					CollectionAssert.AreEqual(buffer, new[] { default(char), 'a', 'b', 'c', default(char) });
					Assert.AreEqual(textReader.Read(), 'd');
					Assert.AreEqual(textReader.Peek(), 'e');
					Assert.AreEqual(textReader.Read(), 'e');
					Assert.AreEqual(textReader.Read(), 'f');
					Assert.AreEqual(textReader.Read(), 'g');
					Assert.AreEqual(textReader.ReadLine(), "line");
					Assert.AreEqual(textReader.ReadToEnd(), "1234" + Environment.NewLine);
					Assert.AreEqual(textReader.Peek(), -1);
					Assert.AreEqual(textReader.Read(), -1);
					Assert.AreEqual(textReader.Peek(), -1);
					Assert.AreEqual(textReader.Read(), -1);
				}
			}
		}
	}
}
