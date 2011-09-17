using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests.MatchTests
{
	public class VariablesInSchemeTests
	{
		[Test]
		public void WhenPatternIsSchemaAndDomainThenAssignSchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("{someprotocol}://acme.com/");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["someprotocol"].Should().Be("http");
		}

		[Test]
		public void WhenSchemeDoesNotMatchThenReturnNull()
		{
			var parsed = ParsedRoutePattern.Parse("ftp://acme.com");
			var url = "http://acme.com".AsUri();
			var actual = parsed.Match(url, null);
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenSchemeMatchAndIsNotVariableThenReturnEmpty()
		{
			var parsed = ParsedRoutePattern.Parse("ftp://acme.com");
			var url = "FTP://acme.com".AsUri();
			var actual = parsed.Match(url, null);
			actual.Should().Be.Empty();
		}
	}
}