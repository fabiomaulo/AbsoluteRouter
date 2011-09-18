using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
{
	public class ParsedRoutePattern
	{
		private static readonly string SchemeDelimiter = Uri.SchemeDelimiter;
		private const char PathDelimiter = '/';
		private const char HostSeparator = '.';
		private const char PortDelimiter = ':';
		private const char QueryDelimiter = '?';
		private const char QuerySeparator = '&';

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
		private IList<string> hostSegments;
		public IEnumerable<string> HostSegments
		{
			get { return hostSegments; }
		}

		private IList<string> pathSegments;
		public IEnumerable<string> PathSegments
		{
			get { return pathSegments; }
		}

		public IEnumerable<KeyValuePair<string, string>> QuerySegments { get; private set; }

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

			string pathAndQuery = !hasPath ? string.Empty: hasDns ? urlCleanedFromScheme.Substring(indexOfFirstSlash + 1) : urlCleanedFromScheme;
			int indexOfQueryStringStart = pathAndQuery.IndexOf(QueryDelimiter);
			PathPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
			QueryPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(indexOfQueryStringStart + 1) : string.Empty;
		}

		private void ExtractSegments()
		{
			// TODO : validation of segments (for example the match-all segment should be the last os a section)
			SchemeSegment = SchemePattern;
			hostSegments = HostPattern == "" ? Enumerable.Empty<string>().ToList() : HostPattern.Split(HostSeparator).ToList();
			pathSegments = PathPattern == "" ? Enumerable.Empty<string>().ToList() : PathPattern.Split(PathDelimiter).ToList();
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
			var parsedUrl = Parse(url.ToString());
			RouteValueDictionary values = defaults != null ? new RouteValueDictionary(defaults) : new RouteValueDictionary();

			// check Scheme
			if (IsVariableSegment(SchemeSegment))
			{
				var variableName = GetVariableName(SchemeSegment);
				values[variableName] = parsedUrl.SchemeSegment;
			}
			else if (!parsedUrl.SchemeSegment.Equals(SchemeSegment, StringComparison.InvariantCultureIgnoreCase) && !"".Equals(SchemeSegment))
			{
				return null;
			}

			// check Host
			if (!SegmentsMatchs(hostSegments, parsedUrl.hostSegments, values, HostSeparator,true) && !"".Equals(HostPattern))
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
			var collectedSegments = 0;
			for (int i = 0; i < segments.Count; i++)
			{
				bool segmenIsAVariable = IsVariableSegment(segments[i]);
				if (IsMatchAllSegment(segments[i]))
				{
					string variableName = GetVariableName(segments[i]);
					var sb = new StringBuilder(100);
					if (segmentesValues.Count <= i)
					{
						if (!segmenIsAVariable)
						{
							return false;
						}
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
					if(!segmenIsAVariable)
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
			if(variableName.StartsWith("*"))
			{
				variableName = variableName.Substring(1);
			}
			return variableName;
		}
	}
}