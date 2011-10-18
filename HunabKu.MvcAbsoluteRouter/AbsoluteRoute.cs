using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
{
	public class AbsoluteRoute : RouteBase
	{
		private RouteValueDictionary constraints;
		private IDictionary<string, Regex> constraintsExpressions;
		private IDictionary<string, Func<string, bool>> constraintsMatchers;
		private ParsedRoutePattern parsedRoute;
		private string urlPattern;

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults = null, RouteValueDictionary constraints = null, RouteValueDictionary dataTokens = null,
		                     IRouteHandler routeHandler = null)
		{
			UrlPattern = urlPattern;
			Defaults = defaults;
			Constraints = constraints;
			DataTokens = dataTokens;
			RouteHandler = routeHandler;
		}

		public RouteValueDictionary Constraints
		{
			get { return constraints; }
			set
			{
				constraints = value;
				CreateConstraintsMatchers();
			}
		}

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

		private void CreateConstraintsMatchers()
		{
			if (constraints == null)
			{
				constraintsMatchers = null;
				constraintsExpressions = null;
				return;
			}
			constraintsMatchers = new Dictionary<string, Func<string, bool>>(constraints.Count);
			constraintsExpressions = new Dictionary<string, Regex>(constraints.Count);
			foreach (var constraint in constraints)
			{
				string parameterName = constraint.Key;
				var constraintsRule = constraint.Value as string;
				if (constraintsRule != null)
				{
					string constraintsRegEx = "^(" + constraintsRule + ")$";
					constraintsExpressions[parameterName] = new Regex(constraintsRegEx, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
					constraintsMatchers[parameterName] = parameterValue => constraintsExpressions[parameterName].IsMatch(parameterValue);
					continue;
				}
				var matcher = constraint.Value as Matchs;
				if(matcher != null)
				{
					constraintsMatchers[parameterName] = matcher.Match;
					continue;
				}
				throw new InvalidOperationException(string.Format("The constraint entry '{0}' on the route with URL pattern '{1}' must have a string value (as Regex) or have to be a MvcAbsoluteRouter.Matchs.", parameterName, UrlPattern));
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
			// Validate the values
			if (!MatchConstraints(values))
			{
				return null;
			}

			var routeData = new RouteData(this, RouteHandler);
			routeData.Values.OverrideMergeWith(values);
			routeData.DataTokens.OverrideMergeWith(DataTokens);

			return routeData;
		}

		private bool MatchConstraints(RouteValueDictionary values)
		{
			return Constraints == null || Constraints.All(constraint => constraintsMatchers[constraint.Key](SafeGetValueAsString(constraint.Key, values)));
		}

		private string SafeGetValueAsString(string parameterName, RouteValueDictionary values)
		{
			object parameterValue;
			values.TryGetValue(parameterName, out parameterValue);
			return Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			Uri requestUrl = requestContext.HttpContext.Request.Url;
			var defaultScheme = requestUrl != null ? requestUrl.Scheme : string.Empty;

			var contextValues = new RouteValueDictionary(requestContext.RouteData.Values);
			if (parsedRoute.HasHostPattern && requestUrl != null)
			{
				// create a fake patterm just to extract variables values from host
				var pattern = (string.IsNullOrEmpty(defaultScheme) ? "http://" : defaultScheme+"://") + parsedRoute.HostPattern;
				var parsedHostRoute = ParsedRoutePattern.Parse(pattern);
				contextValues.MergeWith(parsedHostRoute.Match(requestUrl, null));
			}

			var matchUrl = parsedRoute.CreateUrlWhenMatch(defaultScheme, values, Defaults, contextValues);
			if (matchUrl == null || !MatchConstraints(matchUrl.UsedValues))
			{
				return null;
			}
			var virtualPathData = new VirtualPathData(this, matchUrl.Url);
			virtualPathData.DataTokens.OverrideMergeWith(DataTokens);

			return virtualPathData;
		}
	}
}