using System;
using HunabKu.MvcAbsoluteRouter;
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
}