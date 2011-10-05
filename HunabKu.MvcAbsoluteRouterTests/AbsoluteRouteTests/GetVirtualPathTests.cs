using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests
{
	public class GetVirtualPathTests
	{
		[Test]
		public void WhenPathMatchThenCreateRelativePath()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda/1");
		}

		[Test]
		public void WhenPathContainsCostantsThenCreateRelativePath()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/bubu/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/bubu/1");
		}

		[Test]
		public void WhenHostAndPathMatchThenCreateAbsolutePath()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", id = 1, host="acme" }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("http://acme.com/pizza/calda/1");
		}
	}
}