using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
{
	public class ParsedRoutePattern
	{
		private const char PathDelimiter = '/';
		private const char HostSeparator = '.';
		private const char PortDelimiter = ':';
		private const char QueryDelimiter = '?';
		private const char QuerySeparator = '&';
		private static readonly string SchemeDelimiter = Uri.SchemeDelimiter;
		private IList<string> hostSegments;
		private IList<string> pathSegments;

		private ParsedRoutePattern(string pattern)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}

			if (string.IsNullOrWhiteSpace(pattern))
			{
				throw new ArgumentOutOfRangeException("pattern", "The pattern is empty.");
			}
			OriginalPattern = pattern;
			ExtractPatterns(pattern);
			ExtractSegments();
		}

		public string OriginalPattern { get; private set; }
		public string SchemePattern { get; private set; }
		public string HostPattern { get; private set; }
		public string PathPattern { get; private set; }
		public string QueryPattern { get; private set; }

		public string SchemeSegment { get; private set; }

		public IEnumerable<string> HostSegments
		{
			get { return hostSegments; }
		}

		public IEnumerable<string> PathSegments
		{
			get { return pathSegments; }
		}

		public IEnumerable<KeyValuePair<string, string>> QuerySegments { get; private set; }

		public bool HasHostPattern
		{
			get { return !string.IsNullOrEmpty(HostPattern); }
		}

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}

		private void ExtractPatterns(string urlPattern)
		{
			int indexOfSchemeDelimiter = urlPattern.IndexOf(SchemeDelimiter);
			SchemePattern = string.Empty;
			string urlCleanedFromScheme = urlPattern;
			bool hasScheme = indexOfSchemeDelimiter >= 0;
			if (hasScheme)
			{
				SchemePattern = urlPattern.Substring(0, indexOfSchemeDelimiter);
				urlCleanedFromScheme = urlPattern.Substring(indexOfSchemeDelimiter + SchemeDelimiter.Length);
			}
			int indexOfFirstSlash = urlCleanedFromScheme.IndexOf(PathDelimiter);
			int indexOfFirstDot = urlCleanedFromScheme.IndexOf(HostSeparator);
			bool hasDns = indexOfFirstDot >= 0 || hasScheme;
			bool hasPath = (hasDns && indexOfFirstSlash > indexOfFirstDot) || !hasDns;

			string hostAndPort = !hasDns ? string.Empty : hasPath ? urlCleanedFromScheme.Substring(0, indexOfFirstSlash) : urlCleanedFromScheme;
			int indexOfPortDelimiter = hostAndPort.IndexOf(PortDelimiter);
			HostPattern = indexOfPortDelimiter > 0 ? hostAndPort.Substring(0, indexOfPortDelimiter) : hostAndPort;

			string pathAndQuery = !hasPath ? string.Empty : hasDns ? urlCleanedFromScheme.Substring(indexOfFirstSlash + 1) : urlCleanedFromScheme;
			int indexOfQueryStringStart = pathAndQuery.IndexOf(QueryDelimiter);
			PathPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
			QueryPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(indexOfQueryStringStart + 1) : string.Empty;
		}

		private void ExtractSegments()
		{
			// TODO : validation of segments (for example the match-all segment should be the last of a section)
			SchemeSegment = SchemePattern;
			hostSegments = HostPattern == "" ? new string[0] : HostPattern.Split(HostSeparator).ToArray();
			pathSegments = PathPattern == "" ? new string[0] : PathPattern.Split(PathDelimiter).ToArray();
			QuerySegments = QueryPattern == ""
			                	? Enumerable.Empty<KeyValuePair<string, string>>()
			                	: QueryPattern.Split(QuerySeparator)
			                	  	.Select(x => new Tuple<string, int>(x, x.IndexOf('=')))
			                	  	.Where(t => t.Item2 > 0)
			                	  	.Select(t =>
			                	  	        {
			                	  	        	string varName = t.Item1.Substring(0, t.Item2);
			                	  	        	string varValue = t.Item1.Substring(t.Item2 + 1);
			                	  	        	return new KeyValuePair<string, string>(varName, varValue);
			                	  	        })
			                	  	.ToList();
		}

		public RouteValueDictionary Match(Uri url, RouteValueDictionary defaults)
		{
			if (url == null)
			{
				return null;
			}
			ParsedRoutePattern parsedUrl = Parse(url.ToString());
			RouteValueDictionary values = defaults != null ? new RouteValueDictionary(defaults) : new RouteValueDictionary();

			// check Scheme
			if (IsVariableSegment(SchemeSegment))
			{
				string variableName = GetVariableName(SchemeSegment);
				values[variableName] = parsedUrl.SchemeSegment;
			}
			else if (!parsedUrl.SchemeSegment.Equals(SchemeSegment, StringComparison.InvariantCultureIgnoreCase) && !"".Equals(SchemeSegment))
			{
				return null;
			}

			// check Host
			if (!SegmentsMatchs(hostSegments, parsedUrl.hostSegments, values, HostSeparator, true) && !"".Equals(HostPattern))
			{
				return null;
			}

			// check Path
			if (!SegmentsMatchs(pathSegments, parsedUrl.pathSegments, values, PathDelimiter))
			{
				return null;
			}
			return values;
		}

		private bool SegmentsMatchs(IList<string> segments, IList<string> segmentesValues, RouteValueDictionary values, char segmentsSeparator, bool haveToMatchSegmentsCount = false)
		{
			int collectedSegments = 0;
			for (int i = 0; i < segments.Count; i++)
			{
				bool segmenIsAVariable = IsVariableSegment(segments[i]);
				if (IsMatchAllSegment(segments[i]))
				{
					string variableName = GetVariableName(segments[i]);
					var sb = new StringBuilder(100);
					if (segmentesValues.Count <= i)
					{
						object matchAllDefault;
						if (!values.TryGetValue(variableName, out matchAllDefault))
						{
							return false;
						}
						sb.Append(matchAllDefault);
					}
					else
					{
						sb.Append(segmentesValues[i]);
						collectedSegments++;
						for (int j = i + 1; j < segmentesValues.Count; j++)
						{
							sb.Append(segmentsSeparator).Append(segmentesValues[j]);
							collectedSegments++;
						}
					}
					values[variableName] = sb.ToString();
					break;
				}
				object matchSegment;
				if (segmentesValues.Count <= i)
				{
					if (!segmenIsAVariable)
					{
						return false;
					}
					string variableName = GetVariableName(segments[i]);
					if (!values.TryGetValue(variableName, out matchSegment))
					{
						return false;
					}
				}
				else
				{
					matchSegment = segmentesValues[i];
				}
				if (!segments[i].Equals(matchSegment.ToString(), StringComparison.InvariantCultureIgnoreCase) && !segmenIsAVariable)
				{
					return false;
				}
				if (segmenIsAVariable)
				{
					string variableName = GetVariableName(segments[i]);
					values[variableName] = matchSegment;
				}
				collectedSegments++;
			}
			return !(segments.Count > 0 && haveToMatchSegmentsCount) || collectedSegments >= segmentesValues.Count;
		}

		private bool IsVariableSegment(string urlSegment)
		{
			return urlSegment.StartsWith("{") && urlSegment.EndsWith("}");
		}

		private bool IsMatchAllSegment(string urlSegment)
		{
			return urlSegment.StartsWith("{*") && urlSegment.EndsWith("}");
		}

		private string GetVariableName(string urlSegment)
		{
			string variableName = urlSegment.Trim('{', '}');
			if (variableName.StartsWith("*"))
			{
				variableName = variableName.Substring(1);
			}
			return variableName;
		}

		public string CreateUrlWhenMatch(string defaultScheme, RouteValueDictionary contextValues, RouteValueDictionary defaultValues, RouteValueDictionary values)
		{
			if (values == null)
			{
				values = new RouteValueDictionary();
			}
			if (defaultValues == null)
			{
				defaultValues = new RouteValueDictionary();
			}

			var usedParametersNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string[] hostFilledSegments;
			if(!WhenMatchGetFullFilledSegments(hostSegments, contextValues, defaultValues, usedParametersNames, out hostFilledSegments, true))
			{
				return null;
			}
			string[] pathFilledSegments;
			if (!WhenMatchGetFullFilledSegments(pathSegments, contextValues, defaultValues, usedParametersNames, out pathFilledSegments))
			{
				return null;
			}
			string host = string.Join(HostSeparator.ToString(), hostFilledSegments);
			string path = string.Join(PathDelimiter.ToString(), pathFilledSegments);

			var parametersToUseInQuerystring = new HashSet<string>(values.Keys, StringComparer.OrdinalIgnoreCase);
			parametersToUseInQuerystring.ExceptWith(usedParametersNames);
			string queryString = GetQueryForUnusedParameters(values, parametersToUseInQuerystring);

			return HostSegments.Any()
			       	? (new UriBuilder {Scheme = defaultScheme, Host = host, Path = path, Query = queryString.TrimStart('?')}).ToString()
			       	: path + queryString;
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
						queryStringBuilder.Append(firstParam ? QueryDelimiter : QuerySeparator);
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

		private bool WhenMatchGetFullFilledSegments(IList<string> patternSegments, RouteValueDictionary values, RouteValueDictionary defaults,
		                                                  HashSet<string> usedParametersNames, out string[] filledSegments, bool forceUsageOfDefaultWhereNoValueAvailable = false)
		{
			if(patternSegments.Count == 0)
			{
				filledSegments = new string[0];
				return true;
			}
			List<string> result = new List<string>(50);
			var pendingSubstitutions = new List<string>(10);
			var availableValues = new RouteValueDictionary(values);
			if (forceUsageOfDefaultWhereNoValueAvailable)
			{
				availableValues.MergeWith(defaults);
			}
			foreach (string segment in patternSegments)
			{
				if (IsVariableSegment(segment))
				{
					object actualValue;
					string variableName = GetVariableName(segment);
					if (availableValues.TryGetValue(variableName, out actualValue))
					{
						if (pendingSubstitutions.Count > 0)
						{
							// return pending segments with defaults
							result.AddRange(pendingSubstitutions);
							pendingSubstitutions.Clear();
						}
						usedParametersNames.Add(variableName);
						result.Add(Convert.ToString(actualValue, CultureInfo.InvariantCulture));
					}
					else if (defaults.TryGetValue(variableName, out actualValue))
					{
						usedParametersNames.Add(variableName);
						// enlist the availability of a default
						pendingSubstitutions.Add(Convert.ToString(actualValue, CultureInfo.InvariantCulture));
					}
					else
					{
						// the parameter is required but no value is available: the pattern does not match
						filledSegments= new string[0];
						return false;
					}
				}
				else
				{
					result.Add(segment);
				}
			}
			filledSegments = result.ToArray();
			return true;
		}
	}
}