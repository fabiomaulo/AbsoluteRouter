using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class SchemePatternTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptySchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.SchemePattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenAssignSchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			string actual = parsed.SchemePattern;
			actual.Should().Be("http");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignSchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("https://acme.com/{area}?a=5&b=6");
			string actual = parsed.SchemePattern;
			actual.Should().Be("https");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenAssignSchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("{scheme}://{company}.com/");
			string actual = parsed.SchemePattern;
			actual.Should().Be("{scheme}");
			parsed.HostPattern.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenSchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("ftp://{company}.com");
			string actual = parsed.SchemePattern;
			actual.Should().Be("ftp");
			parsed.HostPattern.Should().Be("{company}.com");
			parsed.PathPattern.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsJustDomainThenEmptySchemePattern()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			string actual = parsed.SchemePattern;
			actual.Should().Be.Empty();
		}
 
	}
}