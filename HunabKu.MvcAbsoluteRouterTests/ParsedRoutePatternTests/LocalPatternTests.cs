using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class LocalPatternTests
	{
		[Test]
 		public void WhenPatternIsJustLocalThenAssignLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.LocalPattern;
			actual.Should().Be("{area}");
		}
	}
}