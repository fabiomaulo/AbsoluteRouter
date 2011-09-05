using System.Collections.Generic;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class HostSegmentsTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptyHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenAssignHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Have.SameSequenceAs("acme","com");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}?a=5&b=6");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Have.SameSequenceAs("acme", "com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenAssignHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Have.SameSequenceAs("{company}", "com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenAssignHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Have.SameSequenceAs("{company}", "com");
		}

		[Test]
		public void WhenPatternIsJustDomainThenAssignHostSegment()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			IEnumerable<string> actual = parsed.HostSegments;
			actual.Should().Have.SameSequenceAs("{company}", "com");
		}
	}
}