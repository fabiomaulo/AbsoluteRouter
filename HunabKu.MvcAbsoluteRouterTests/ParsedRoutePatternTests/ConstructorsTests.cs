using System;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class ConstructorsTests
	{
		[Test]
 		public void WhenEmptyPatternThenThrows()
		{
			Executing.This(() => ParsedRoutePattern.Parse(null)).Should().Throw<ArgumentNullException>();
		}
	}

	public class ParsedRoutePattern
	{
		private ParsedRoutePattern(string pattern)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
		}

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}
	}
}