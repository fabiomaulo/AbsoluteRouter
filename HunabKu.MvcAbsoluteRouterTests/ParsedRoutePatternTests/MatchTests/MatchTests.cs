using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests.MatchTests
{
	public class MatchTests
	{
		[Test]
		public void WhenNoMatchThenReturnNull()
		{
			var parsed = ParsedRoutePattern.Parse("www.acme.com");
			var url = "http://www.pizza.com".AsUri();
			RouteValueDictionary defaults = null;
			RouteValueDictionary actual = parsed.Match(url, defaults);
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenNullThenReturnNull()
		{
			var parsed = ParsedRoutePattern.Parse("www.acme.com");
			RouteValueDictionary actual = parsed.Match(null, null);
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenMatchWithoutVariableThenEmpty()
		{
			var parsed = ParsedRoutePattern.Parse("www.acme.com");
			var url = "http://www.acme.com".AsUri();
			RouteValueDictionary defaults = null;
			RouteValueDictionary actual = parsed.Match(url, defaults);
			actual.Should().Not.Be.Null();
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenMatchWithoutVariableAndDafaulsThenReturnDefaults()
		{
			var parsed = ParsedRoutePattern.Parse("www.acme.com");
			var url = "http://www.acme.com".AsUri();
			RouteValueDictionary actual = parsed.Match(url, new RouteValueDictionary(new {pizza=1 }));
			actual.Should().Not.Be.Empty();
			actual.Keys.Should().Contain("pizza");
			actual.Values.Should().Contain(1);
		}
	}
}