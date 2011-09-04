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

		[Test]
		public void WhenPatternContainsDomainThenAssignLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			string actual = parsed.LocalPattern;
			actual.Should().Be("{area}");
		}

		[Test]
		public void WhenPatternContainsQueryThenAssignLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}?a=5&b=6");
			string actual = parsed.LocalPattern;
			actual.Should().Be("{area}");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}?a=5&b=6");
			string actual = parsed.LocalPattern;
			actual.Should().Be("{area}");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenEmptyLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			string actual = parsed.LocalPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenEmptyLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com");
			string actual = parsed.LocalPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsJustDomainThenEmptyLocalPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			string actual = parsed.LocalPattern;
			actual.Should().Be.Empty();
		}
	}
}