using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests
{
	public class DnsSafeHostPatternTests
	{
		[Test]
		public void WhenPatternIsJustLocalThenEmptyDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{area}");
			string actual = parsed.HostPattern;
			actual.Should().Be.Empty();
		}

		[Test]
		public void WhenPatternContainsDomainThenAssignDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}");
			string actual = parsed.HostPattern;
			actual.Should().Be("acme.com");
		}

		[Test]
		public void WhenPatternContainsDomainAndQueryThenAssignDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://acme.com/{area}?a=5&b=6");
			string actual = parsed.HostPattern;
			actual.Should().Be("acme.com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainThenAssignDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			string actual = parsed.HostPattern;
			actual.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithoutEndSlashThenAssignDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com");
			string actual = parsed.HostPattern;
			actual.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsJustDomainThenAssignDnsSafeHostPattern()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com");
			string actual = parsed.HostPattern;
			actual.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsSchemaAndDomainWithPortThenAssignDnsSafeHostPatternWithoutPort()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com:81/");
			string actual = parsed.HostPattern;
			actual.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsJustDomainWithPortThenAssignDnsSafeHostPatternWithoutPort()
		{
			var parsed = ParsedRoutePattern.Parse("{company}.com:81");
			string actual = parsed.HostPattern;
			actual.Should().Be("{company}.com");
		}

		[Test]
		public void WhenPatternIsMatchAllDomainWithPortThenAssignDnsSafeHostPatternWithoutPort()
		{
			var parsed = ParsedRoutePattern.Parse("http://{*domain}:81");
			string actual = parsed.HostPattern;
			actual.Should().Be("{*domain}");
		}
	}
}