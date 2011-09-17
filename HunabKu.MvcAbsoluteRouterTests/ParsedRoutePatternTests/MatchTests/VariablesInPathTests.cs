using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests.MatchTests
{
	public class VariablesInPathTests
	{
		[Test, Ignore("Not fixed yet")]
		public void WhenPatternContainsDomainThenReturnVariableInPath()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["area"].Should().Be("pizza");
		}
 
	}
}