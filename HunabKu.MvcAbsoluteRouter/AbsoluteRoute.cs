using System;
using System.Web;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
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
			Uri requestUrl = httpContext.Request.Url;
			RouteValueDictionary values = parsedRoute.Match(requestUrl, Defaults);
			if (values == null)
			{
				return null;
			}

			var routeData = new RouteData(this, RouteHandler);
			foreach (var value in values)
			{
				routeData.Values.Add(value.Key, value.Value);
			}
			if (DataTokens != null)
			{
				foreach (var token in DataTokens)
				{
					routeData.DataTokens[token.Key] = token.Value;
				}
			}

			return routeData;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			return null;
		}
	}
}