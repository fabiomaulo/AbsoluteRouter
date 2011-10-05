using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
{
	public class StringConstraintsTests
	{
		[Test]
		public void WhenNoMatchByConstraintsThenNull()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}", constraints: new RouteValueDictionary(new { controller = "nomatch" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", id = 1 }));
			virtualPath.Should().Be.Null();
		}

		[Test]
		public void WhenMatchThenAssignValues()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}", constraints: new RouteValueDictionary(new { controller = "pazza|pizza" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda/1");
		}

		[Test]
		public void WhenNoMatchByConstraintsWithValuesInRouteDataThenNull()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routeData = new RouteData();
			routeData.Values.Add("controller", "pizza");
			var requestContext = new RequestContext(context, routeData);

			var route = new AbsoluteRoute("{controller}/{action}/{id}", constraints: new RouteValueDictionary(new { controller = "nomatch" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "calda", id = 1 }));
			virtualPath.Should().Be.Null();
		}

		[Test]
		public void WhenMatchWithValuesInRouteDataThenAssignValues()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routeData = new RouteData();
			routeData.Values.Add("controller", "pizza");
			var requestContext = new RequestContext(context, routeData);

			var route = new AbsoluteRoute("{controller}/{action}/{id}", constraints: new RouteValueDictionary(new { controller = "pazza|pizza" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "calda", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda/1");
		}
	}
}