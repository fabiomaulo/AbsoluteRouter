using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests
{
	public class GetRouteDataBaseTests
	{
		[Test]
 		public void WhenNoMatchThenNull()
		{
			var route = new AbsoluteRoute("fixedvalue/{action}/{id}");
			var context = "http://acme.com/pizza".AsUri().ToHttpContext();
			route.GetRouteData(context).Should().Be.Null();
		}

		[Test]
		public void WhenMatchThenAssignValues()
		{
			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routedata = route.GetRouteData(context);
			routedata.Values.Should().Not.Be.Null();
			routedata.Values["controller"].Should().Be("pizza");
			routedata.Values["action"].Should().Be("calda");
			routedata.Values["id"].Should().Be("1");
		}

		[Test]
		public void WhenMatchThenAssignRoute()
		{
			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routedata = route.GetRouteData(context);
			routedata.Route.Should().Be.SameInstanceAs(route);
		}

		[Test]
		public void WhenMatchThenAssignRouteHandler()
		{
			var stopRoutingHandler = new StopRoutingHandler();

			var route = new AbsoluteRoute("{controller}/{action}/{id}",routeHandler:stopRoutingHandler);
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routedata = route.GetRouteData(context);
			routedata.RouteHandler.Should().Be.SameInstanceAs(stopRoutingHandler);
		}

		[Test]
		public void WhenMatchThenCopyDataTokens()
		{
			var route = new AbsoluteRoute("{controller}/{action}/{id}", dataTokens: new RouteValueDictionary(new { a = 1, b = 2 }));
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routedata = route.GetRouteData(context);
			var tokens = routedata.DataTokens;
			tokens.Should().Not.Be.Null();
			tokens["a"].Should().Be(1);
			tokens["b"].Should().Be(2);
		}

		[Test]
		public void WhenNoMatchWithQueryStringThenNull()
		{
			var route = new AbsoluteRoute("http://{*host}/{category}");
			var context = "http://acme.com/?p=5".AsUri().ToHttpContext();
			var routedata = route.GetRouteData(context);
			routedata.Should().Be.Null();
		}

	}
}