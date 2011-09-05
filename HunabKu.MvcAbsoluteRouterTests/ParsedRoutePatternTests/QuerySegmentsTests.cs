using System.Collections.Generic;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class QuerySegmentsTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsQueryThenAssignQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}?a=5&b=6");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Have.SameSequenceAs(new KeyValuePair<string, string>("a", "5"), new KeyValuePair<string, string>("b", "6"));
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}?a={value1}&b={value2}");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Have.SameSequenceAs(new KeyValuePair<string, string>("a", "{value1}"), new KeyValuePair<string, string>("b", "{value2}"));
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternIsJustDomainThenEmptyQueryPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			IEnumerable<KeyValuePair<string, string>> actual = parsed.QuerySegments;
			actual.Should().Be.Empty();
		}
	}
}