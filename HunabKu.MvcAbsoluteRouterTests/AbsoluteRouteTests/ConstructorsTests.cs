using System;
using System.Web;
using System.Web.Routing;
using HunabKu.MvcAbsoluteRouter;

namespace HunabKu.MvcAbsoluteRouterTests.AbsoluteRouteTests
{
	public class AbsoluteRoute : RouteBase
	{
		private string urlPattern;
		private ParsedRoutePattern parsedRoute;

		public AbsoluteRoute(string urlPattern, IRouteHandler routeHandler)
		{
			UrlPattern = urlPattern;
			RouteHandler = routeHandler;
		}

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults, IRouteHandler routeHandler)
		{
			UrlPattern = urlPattern;
			Defaults = defaults;
			RouteHandler = routeHandler;
		}

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
		{
			UrlPattern = urlPattern;
			Defaults = defaults;
			Constraints = constraints;
			RouteHandler = routeHandler;
		}

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
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

		public virtual AbsolutePathData GetAbsolutePath(RequestContext requestContext, RouteValueDictionary values)
		{
			throw new NotImplementedException();
		}
	}

	public class AbsolutePathData
	{
		private readonly RouteValueDictionary dataTokens = new RouteValueDictionary();

		public AbsolutePathData(RouteBase route, Uri uri)
		{
			Route = route;
			AbsolutePath = uri;
		}

		public RouteValueDictionary DataTokens
		{
			get { return dataTokens; }
		}

		public RouteBase Route { get; set; }

		public Uri AbsolutePath { get; set; }
	}

	public class ConstructorsTests {}
}