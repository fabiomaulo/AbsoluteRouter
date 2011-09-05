using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class SchemeSegmentTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptySchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.SchemeSegment;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenAssignSchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			string actual = parsed.SchemeSegment;
			actual.Should().Be("http");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignSchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("https://acme.com/{area}?a=5&b=6");
			string actual = parsed.SchemeSegment;
			actual.Should().Be("https");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenAssignSchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("{scheme}://{company}.com/");
			string actual = parsed.SchemeSegment;
			actual.Should().Be("{scheme}");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenSchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("ftp://{company}.com");
			string actual = parsed.SchemeSegment;
			actual.Should().Be("ftp");
		}

		[Test]
		public void WhenPatternIsJustDomainThenEmptySchemeSegment()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			string actual = parsed.SchemeSegment;
			actual.Should().Be.Empty();
		}
	}
}