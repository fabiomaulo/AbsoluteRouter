using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests.MatchTests
{
	public class VariablesInPathTests
	{
		[Test]
		public void WhenPatternContainsDomainThenReturnVariableInPath()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["area"].Should().Be("pizza");
		}

		[Test]
		public void WhenUrlIsLargerThenMatchAndReturnVariableInPath()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			var url = "http://acme.com/pizza/5".AsUri();
			var actual = parsed.Match(url, null);
			actual["area"].Should().Be("pizza");
		}

		[Test]
		public void WhenPatternIsLargerAndHasDefaultThenMatchAndReturnVariableInPath()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}/{id}");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, new RouteValueDictionary(new { id = 50 }));
			actual["area"].Should().Be("pizza");
			actual["id"].Should().Be(50);
		}

		[Test]
		public void WhenPatternIsLargerDoesNotHaveDefaultThenNoMatch()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}/{id}");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, new RouteValueDictionary(new { ostia = 50 }));
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenPatternWithoutDomainAndIsLargerAndHasDefaultThenMatchAndReturnVariableInPath()
		{
			var parsed = ParsedRoutePattern.Parse("{area}/{id}");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, new RouteValueDictionary(new { id = 50 }));
			actual["area"].Should().Be("pizza");
			actual["id"].Should().Be(50);
		}

		[Test]
		public void WhenMatchAllThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}/{*info}");
			var url = "http://acme.com/pippo/pluto/paperino".AsUri();
			var actual = parsed.Match(url, new RouteValueDictionary(new { id = 50 }));
			actual["area"].Should().Be("pippo");
			actual["info"].Should().Be("pluto/paperino");
		}

		[Test]
		public void WhenMatchAllHasJustDefaultThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{*info}");
			var url = "http://acme.com".AsUri();
			var actual = parsed.Match(url, new RouteValueDictionary(new { info = 50 }));
			actual["info"].Should().Be("50");
		}

		[Test]
		public void WhenMatchAllHasNoValueThenNoMatch()
		{
			var parsed = ParsedRoutePattern.Parse("acme.com/{*info}");
			var url = "http://acme.com".AsUri();
			var actual = parsed.Match(url, null);
			actual.Should().Be.Null();
		}
	}
}