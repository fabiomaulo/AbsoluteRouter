using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;
using SharpTestsEx.Mvc;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests.VirtualPathTests
{
	public class NoMatchsTests
	{
		[Test]
		public void WhenPathWithRequiredParamWithoutValueThenNull()
		{
			var context = "http://acme.com".AsUri().ToHttpContext();
			var requestContext = new RequestContext(context, new RouteData());

			var route = new AbsoluteRoute("{controller}/{action}/{id}");
			var virtualPath = route.GetVirtualPath(requestContext, new RouteValueDictionary(new { controller = "pizza", id = 1 }));
			virtualPath.Should().Be.Null();
		}
	}
}