using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
{
	public class CreationOfHostTests
	{
		[Test]
		public void WhenHostHasCatchAllWithNoValueThenUseContextAsDefault()
		{
			var context = "http://ar.acme.com/pizza".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{topLevelDomain}.{*domain}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { topLevelDomain = "cl" }));
			virtualPath.VirtualPath.Should().Be("http://cl.acme.com/");
		}

		[Test]
		public void WhenHostHasCatchAllWithValueThenUseValue()
		{
			var context = "http://ar.acme.com/pizza".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{topLevelDomain}.{*domain}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { topLevelDomain = "cl", domain="google.net" }));
			virtualPath.VirtualPath.Should().Be("http://cl.google.net/");
		}
	}
}