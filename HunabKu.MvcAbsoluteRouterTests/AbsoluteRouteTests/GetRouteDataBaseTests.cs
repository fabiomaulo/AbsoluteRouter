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
	}
}