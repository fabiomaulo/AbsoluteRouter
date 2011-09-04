using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class QueryPatternTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.QueryPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			string actual = parsed.QueryPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsQueryThenAssignQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}?a=5&b=6");
			string actual = parsed.QueryPattern;
			actual.Should().Be("a=5&b=6");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}?a={value1}&b={value2}");
			string actual = parsed.QueryPattern;
			actual.Should().Be("a={value1}&b={value2}");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			string actual = parsed.QueryPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com");
			string actual = parsed.QueryPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsJustDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			string actual = parsed.QueryPattern;
			actual.Should().Be.Empty();
		}
	}
}