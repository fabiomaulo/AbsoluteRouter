using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
{
	public class UsageOfDefaultsTests
	{
		[Test]
		public void WhenHostMatchAndRouteHasDefaultsThenCreateAbsolutePathWithoutNotNeededDefaults()
		{
			// when has default and values are not explicitly defined then does not use those values to create the URL
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "acme" }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/");
		}

		[Test]
		public void WhenHostMatchAndRouteHasDefaultsAndLastValueThenCreateAbsolutePathWithDefaultsAndValues()
		{
			// when has default and some values are explicitly defined then use defaults and values to create the URL
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "acme", action="Edit" }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/Home/Edit");
		}

		[Test]
		public void WhenHostMatchAndRouteHasDefaultsAndFirstValueThenCreateAbsolutePathWithoutNotNeededDefaults()
		{
			// when has default and some values are explicitly defined then use defaults and values to create the URL
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "acme", controller = "Catalog" }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/Catalog");
		}

		[Test]
		public void WhenHostHasDefaultsThenUseDefaults()
		{
			// when has default and some values are explicitly defined then use defaults and values to create the URL
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new {host="acme", controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, null);
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://acme.com/");
		}

		[Test]
		public void WhenHostHasDefaultsAndValuesThenUseValues()
		{
			// when has default and some values are explicitly defined then use defaults and values to create the URL
			var context = "https://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("http://{host}.com/{controller}/{action}", defaults: new RouteValueDictionary(new { host = "acme", controller = "Home", action = "Index" }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { host = "bubu"}));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://bubu.com/");
		}

		[Test]
		public void WhenHasDefaultsAndValuesAndContextThenDoesNotUseContextForNotRequiredParameters()
		{
			// when has default and some values are explicitly defined then use defaults and values to create the URL
			var context = "https://something.acme.com/Super/Action/1234".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{subdomain}.acme.com/{controller}/{action}/{id}", defaults: new RouteValueDictionary(new { controller = "Home", action = "Index", id= string.Empty }));
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "Pizza" }));
			virtualPath.Route.Should().Be.SameInstanceAs(route);
			virtualPath.VirtualPath.Should().Be("https://something.acme.com/Pizza");
		}
	}
}