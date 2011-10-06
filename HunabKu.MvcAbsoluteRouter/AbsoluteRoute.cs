using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
{
	public class AbsoluteRoute : RouteBase
	{
		private string urlPattern;
		private ParsedRoutePattern parsedRoute;
		private RouteValueDictionary constraints;
		private IDictionary<string, Func<string, bool>> constraintsMatchers;
		private IDictionary<string, Regex> constraintsExpressions;

		public AbsoluteRoute(string urlPattern, RouteValueDictionary defaults = null, RouteValueDictionary constraints = null, RouteValueDictionary dataTokens = null, IRouteHandler routeHandler = null)
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

		private void CreateConstraintsMatchers()
		{
			if(constraints == null)
			{
				constraintsMatchers = null;
				constraintsExpressions= null;
				return;
			}
			constraintsMatchers = new Dictionary<string, Func<string, bool>>(constraints.Count);
			constraintsExpressions = new Dictionary<string, Regex>(constraints.Count);
			foreach (var constraint in constraints)
			{
				var parameterName = constraint.Key;
				var constraintsRule = constraint.Value as string;
				if (constraintsRule == null)
				{
					throw new InvalidOperationException(string.Format("The constraint entry '{0}' on the route with URL pattern '{1}' must have a string value.", parameterName, UrlPattern));
				}
				string constraintsRegEx = "^(" + constraintsRule + ")$";
				constraintsExpressions[parameterName] = new Regex(constraintsRegEx, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
				constraintsMatchers[parameterName] = parameterValue => constraintsExpressions[parameterName].IsMatch(parameterValue);
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
			if (!MatchConstraints(values))
			{
				return null;
			}

			OverrideMergeDictionary(values, routeData.Values);
			OverrideMergeDictionary(DataTokens, routeData.DataTokens);

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

		private static void MergeDictionary(RouteValueDictionary source, RouteValueDictionary destination)
		{
			if (source == null)
			{
				return;
			}
			foreach (var value in source.Where(x=> !destination.ContainsKey(x.Key)))
			{
				destination[value.Key] = value.Value;
			}
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			if (values == null)
			{
				values = new RouteValueDictionary();
			}
			var defaultValues = Defaults ?? new RouteValueDictionary();
			var contextValues = new RouteValueDictionary(requestContext.RouteData.Values);
			OverrideMergeDictionary(values, contextValues);
			if (!MatchConstraints(contextValues))
			{
				return null;
			}

			string defaultScheme= string.Empty;
			if (requestContext.HttpContext.Request.Url != null)
			{
				defaultScheme = requestContext.HttpContext.Request.Url.Scheme;
			}
			HashSet<string> usedParametersNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var hostSegments = GetFullFilledSegments(parsedRoute.HostSegments, contextValues, defaultValues, usedParametersNames, true).ToArray();
			var pathSegments = GetFullFilledSegments(parsedRoute.PathSegments, contextValues, defaultValues, usedParametersNames).ToArray();
			bool hasUnreachableParameter = hostSegments.Concat(pathSegments).Any(x=> IsVariableSegment(x));
			if(hasUnreachableParameter)
			{
				return null;
			}
			string host = string.Join(".", hostSegments);
			string path = string.Join("/", pathSegments);
			
			var parametersToUseInQuerystring = new HashSet<string>(values.Keys, StringComparer.OrdinalIgnoreCase);
			parametersToUseInQuerystring.ExceptWith(usedParametersNames);
			string queryString= GetQueryForUnusedParameters(values, parametersToUseInQuerystring);
			string virtualPath = parsedRoute.HostSegments.Any()
			                     	? (new UriBuilder {Scheme = defaultScheme, Host = host, Path = path, Query = queryString.TrimStart('?')}).ToString()
			                     	: path + queryString;

			var virtualPathData = new VirtualPathData(this, virtualPath);
			OverrideMergeDictionary(DataTokens, virtualPathData.DataTokens);

			return virtualPathData;
		}

		private string GetQueryForUnusedParameters(RouteValueDictionary values, HashSet<string> parametersToUseInQuerystring)
		{
			if (parametersToUseInQuerystring.Count > 0)
			{
				var queryStringBuilder = new StringBuilder(512);
				bool firstParam = true;
				foreach (string unusedNewValue in parametersToUseInQuerystring)
				{
					object value;
					if (values.TryGetValue(unusedNewValue, out value))
					{
						queryStringBuilder.Append(firstParam ? '?' : '&');
						firstParam = false;
						queryStringBuilder.Append(Uri.EscapeDataString(unusedNewValue));
						queryStringBuilder.Append('=');
						queryStringBuilder.Append(Uri.EscapeDataString(Convert.ToString(value, CultureInfo.InvariantCulture)));
					}
				}
				return queryStringBuilder.ToString();
			}
			return string.Empty;
		}

		private IEnumerable<string> GetFullFilledSegments(IEnumerable<string> patternSegments, RouteValueDictionary values, RouteValueDictionary defaults, HashSet<string> usedParametersNames, bool forceUsageOfDefaultWhereNoValueAvailable = false)
		{
			List<string> pendingSubstitutions = new List<string>(10);
			var availableValues = new RouteValueDictionary(values);
			if (forceUsageOfDefaultWhereNoValueAvailable)
			{
				MergeDictionary(defaults, availableValues);
			}
			foreach (var segment in patternSegments)
			{
				if (IsVariableSegment(segment))
				{
					object actualValue;
					var variableName = GetVariableName(segment);
					if (availableValues.TryGetValue(variableName, out actualValue))
					{
						if (pendingSubstitutions.Count > 0)
						{
							// return pending segments with defaults
							foreach (var pendingSubstitution in pendingSubstitutions)
							{
								yield return pendingSubstitution;
							}
							pendingSubstitutions.Clear();
						}
						usedParametersNames.Add(variableName);
						var actualValueString = Convert.ToString(actualValue, CultureInfo.InvariantCulture);
						yield return actualValueString;
					}
					else if (defaults.TryGetValue(variableName, out actualValue))
					{
						usedParametersNames.Add(variableName);
						// enlist the availability of a default
						pendingSubstitutions.Add(Convert.ToString(actualValue, CultureInfo.InvariantCulture));
					}
					else
					{
						yield return segment;
					}
				}
				else
				{
					yield return segment;
				}
			}
		}

		private bool IsVariableSegment(string urlSegment)
		{
			return urlSegment.StartsWith("{") && urlSegment.EndsWith("}");
		}

		private string GetVariableName(string urlSegment)
		{
			return urlSegment.Trim('{', '}');
		}

	}
}