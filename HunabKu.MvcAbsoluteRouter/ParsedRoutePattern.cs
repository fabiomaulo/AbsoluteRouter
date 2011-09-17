using System;
using System.Collections.Generic;
using System.Linq;
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
			if (indexOfSchemeDelimiter >= 0)
			{
				SchemePattern = urlPattern.Substring(0, indexOfSchemeDelimiter);
				urlCleanedFromScheme = urlPattern.Substring(indexOfSchemeDelimiter + SchemeDelimiter.Length);
			}
			int indexOfFirstSlash = urlCleanedFromScheme.IndexOf(PathDelimiter);
			int indexOfFirstDot = urlCleanedFromScheme.IndexOf(HostSeparator);
			bool hasDns = indexOfFirstDot >= 0;
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

			if (IsVariableSegment(SchemeSegment))
			{
				var variableName = GetVariableName(SchemeSegment);
				values[variableName] = parsedUrl.SchemeSegment;
			}
			else if (!parsedUrl.SchemeSegment.Equals(SchemeSegment) && SchemeSegment != "")
			{
				return null;
			}
			if(hostSegments.Count != parsedUrl.hostSegments.Count)
			{
				return null;
			}
			for (int i = 0; i < hostSegments.Count; i++)
			{
				bool segmenIsAVariable = IsVariableSegment(hostSegments[i]);
				string matchSegment = parsedUrl.hostSegments[i];
				if (!matchSegment.Equals(hostSegments[i], StringComparison.InvariantCultureIgnoreCase) && !segmenIsAVariable)
				{
					return null;
				}
				if (segmenIsAVariable)
				{
					string variableName = GetVariableName(hostSegments[i]);
					values[variableName] = matchSegment;
				}
			}
			for (int i = 0; i < pathSegments.Count; i++)
			{
				bool segmenIsAVariable = IsVariableSegment(pathSegments[i]);
				string matchSegment = parsedUrl.pathSegments[i];
				if (!matchSegment.Equals(pathSegments[i], StringComparison.InvariantCultureIgnoreCase) && !segmenIsAVariable)
				{
					return null;
				}
				if (segmenIsAVariable)
				{
					string variableName = GetVariableName(pathSegments[i]);
					values[variableName] = matchSegment;
				}
			}
			return values;
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