using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
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
		public void WhenPathMatchWithValuesInRouteDataThenAssignValues()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routeData = new RouteData();
			routeData.Values.Add("controller", "pizza");
			var requestContext = new RequestContext(context, routeData);

			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { action = "calda", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("pizza/calda/1");
		}

		[Test]
		public void WhenPathMatchThenOverrideValuesInRouteData()
		{
			var context = "http://acme.com/pizza/calda/1".AsUri().ToHttpContext();
			var routeData = new RouteData();
			routeData.Values.Add("controller", "pizza");
			var requestContext = new RequestContext(context, routeData);

			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller="tepiso", action = "calda", id = 1 }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("tepiso/calda/1");
		}

		[Test]
		public void WhenPathMatchCaseInsensitiveThenCreateRelativePath()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { Controller = "pizza", Action = "calda", ID = 1 }));
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

		[Test]
		public void WhenHostMatchThenCreateAbsolutePathWithSchemeFromContext()
		{
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{host}.com/{controller}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host="acme", controller = "pizza"}));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/pizza");
		}

		[Test]
		public void WhenMatchRouteContainsDataTokensThenCopyDataTokens()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}", dataTokens: new RouteValueDictionary(new { a = 1, b = 2 }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", action = "calda", id = 1 }));
			var tokens = virtualPath.DataTokens;
			tokens.Should().Not.Be.Null();
			tokens["a"].Should().Be(1);
			tokens["b"].Should().Be(2);
		}

		[Test]
		public void WhenHostMatchAndRouteHasDefaultsThenCreateAbsolutePathWithDefaults()
		{
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "acme"}));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/Home/Index");
		}
	}
}