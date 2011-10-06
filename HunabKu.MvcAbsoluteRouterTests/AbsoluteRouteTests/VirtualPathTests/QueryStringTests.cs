using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
{
	public class QueryStringTests
	{
		[Test]
		public void WhenPathMatchAndHasAdditionalValueThenCreateRelativePathWithQueryString()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", currentPage = 2 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda?currentPage=2");
		}

		[Test]
		public void WhenPathMatchAndHasAdditionalValuesThenCreateRelativePathWithQueryString()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", topLevelDomain = "xx", currentPage = 2 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda?topLevelDomain=xx&currentPage=2");
		}


		[Test]
		public void WhenHostMatchThenCreateAbsolutePathWithSchemeFromContext()
		{
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { controller = "Home", action="Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "acme", page = 10 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/?page=10");
		}

	}
}