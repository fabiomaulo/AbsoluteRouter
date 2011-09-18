using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.ParsedRoutePatternTests.MatchTests
{
	public class VariablesInDomainTests
	{
		[Test]
		public void WhenDomainContainsVariableAndMatchThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["company"].Should().Be("acme");
		}

		[Test]
		public void WhenDomainContainsVariableAndNoMatchThenReturnNull()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			var url = "http://acme.org/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenDomainContainsVariableAndMatchCaseInsensitiveThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.cOM/");
			var url = "http://acme.CoM/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["company"].Should().Be("acme");
		}

		[Test]
		public void WhenDomainLargerThenNoMatch()
		{
			var parsed = ParsedRoutePattern.Parse("http://{company}.com/");
			var url = "http://acme.com.ar".AsUri();
			var actual = parsed.Match(url, null);
			actual.Should().Be.Null();
		}

		[Test]
		public void WhenDomainIsMatchAllThenReturnVariableValue()
		{
			var parsed = ParsedRoutePattern.Parse("http://{*domain}/");
			var url = "http://acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["domain"].Should().Be("acme.com");
		}

		[Test]
		public void WhenDomainHasMatchAllThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://{topLevelDomain}.{*domain}");
			var url = "http://ar.acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["topLevelDomain"].Should().Be("ar");
			actual["domain"].Should().Be("acme.com");
		}

		[Test]
		public void WhenUrlWithoutSchemeAndDomainHasMatchAllThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("{topLevelDomain}.{*domain}");
			var url = "http://ar.acme.com/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["topLevelDomain"].Should().Be("ar");
			actual["domain"].Should().Be("acme.com");
		}

		[Test]
		public void WhenMatchAllHasJustOneValuesThenReturnVariablesValues()
		{
			var parsed = ParsedRoutePattern.Parse("http://{topLevelDomain}.{*domain}");
			var url = "http://ar.acme/pizza".AsUri();
			var actual = parsed.Match(url, null);
			actual["topLevelDomain"].Should().Be("ar");
			actual["domain"].Should().Be("acme");
		}
	}
}