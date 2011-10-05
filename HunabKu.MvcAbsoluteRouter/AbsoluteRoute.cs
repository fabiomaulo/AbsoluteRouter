using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
			// Validate the values
			if (!MatchConstraints(values, RouteDirection.IncomingRequest))
			{
				return null;
			}

			OverrideMergeDictionary(values, routeData.Values);
			OverrideMergeDictionary(DataTokens, routeData.DataTokens);

			return routeData;
		}

		private bool MatchConstraints(RouteValueDictionary values, RouteDirection routeDirection)
		{
			return Constraints == null || Constraints.All(constraint => MatchConstraint(constraint.Value, constraint.Key, values, routeDirection));
		}

		private bool MatchConstraint(object constraint, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			// Treat the constraint as Regex template.
			var constraintsRule = constraint as string;
			if (constraintsRule == null)
			{
				throw new InvalidOperationException(string.Format("The constraint entry '{0}' on the route with URL pattern '{1}' must have a string value.", parameterName, UrlPattern));
			}

			object parameterValue;
			values.TryGetValue(parameterName, out parameterValue);
			string parameterValueString = Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
			string constraintsRegEx = "^(" + constraintsRule + ")$";
			return Regex.IsMatch(parameterValueString, constraintsRegEx,
					RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		private static void OverrideMergeDictionary(RouteValueDictionary source, RouteValueDictionary destination)
		{
			if (source == null)
			{
				return;
			}
			foreach (var value in source)
			{
				destination[value.Key] = value.Value;
			}
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			return null;
		}
	}
}