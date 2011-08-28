using System;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class ConstructorsTests
	{
		[Test]
 		public void WhenNullPatternThenThrows()
		{
			Executing.This(() => ParsedRoutePattern.Parse(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenEmptyPatternThenThrows()
		{
			Executing.This(() => ParsedRoutePattern.Parse("")).Should().Throw<ArgumentOutOfRangeException>();
			Executing.This(() => ParsedRoutePattern.Parse("   ")).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenValidPatternThenAssignOrifinal()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.OriginalPattern;
			actual.Should().Be("{area}");
		}
	}
}