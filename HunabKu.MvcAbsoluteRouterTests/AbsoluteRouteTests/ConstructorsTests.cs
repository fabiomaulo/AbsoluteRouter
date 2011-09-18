using System;
using System.Web;
using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;
using NUnit.Framework;
using SharpTestsEx;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests
{
	public class AbsoluteRoute : RouteBase
	{
		private string urlPattern;
		private ParsedRoutePattern parsedRoute;

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults = null, RouteValueDictionary constraints = null, RouteValueDictionary dataTokens = null, IRouteHandler routeHandler = null)
		{
			UrlPattern = urlPattern;
			Defaults = defaults;
			Constraints = constraints;
			DataTokens = dataTokens;
			RouteHandler = routeHandler;
		}

		public RouteValueDictionary Constraints { get; set; }

		public RouteValueDictionary DataTokens { get; set; }

		public RouteValueDictionary Defaults { get; set; }

		public IRouteHandler RouteHandler { get; set; }

		public string UrlPattern
		{
			get { return urlPattern ?? string.Empty; }
			set
			{
				parsedRoute = ParsedRoutePattern.Parse(value);
				urlPattern = value;
			}
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			throw new NotImplementedException();
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			throw new NotImplementedException();
		}
	}

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
	}
}