using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests
{
	public class ConstructorsTests
	{
		[Test]
		public void WhenNullUrlThenThrows()
		{
			Executing.This(()=> new AbsoluteRoute(null)).Should().Throw();
		}

		[Test]
		public void WhenUrlPatternHasValuesThenSetValue()
		{
			string pattern = "{controller}/{action}/{id}";
			var route = new AbsoluteRoute(pattern);
			route.UrlPattern.Should().Be(pattern);
		}

		[Test]
		public void WhenUseRouteHandlerThenSetValue()
		{
			string pattern = "{controller}/{action}/{id}";
			var stopRoutingHandler = new StopRoutingHandler();
			var route = new AbsoluteRoute(pattern, routeHandler:stopRoutingHandler);
			route.RouteHandler.Should().Be(stopRoutingHandler);
		}
	}
}