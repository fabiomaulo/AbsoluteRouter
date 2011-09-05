using System;
using System.Collections.Generic;
using System.Linq;

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
		public string LocalPattern { get; private set; }
		public string QueryPattern { get; private set; }

		public string SchemeSegment { get; private set; }
		public IEnumerable<string> HostSegments { get; private set; }
		public IEnumerable<string> LocalSegments { get; private set; }
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
			LocalPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
			QueryPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(indexOfQueryStringStart + 1) : string.Empty;
		}

		private void ExtractSegments()
		{
			SchemeSegment = SchemePattern;
			HostSegments = HostPattern == "" ? Enumerable.Empty<string>() : HostPattern.Split(HostSeparator);
			LocalSegments = LocalPattern == "" ? Enumerable.Empty<string>() : LocalPattern.Split(PathDelimiter);
			QuerySegments = QueryPattern == "" ? Enumerable.Empty<KeyValuePair<string, string>>() : QueryPattern.Split(QuerySeparator).Where(varAssign => varAssign.IndexOf('=') > 0).Select(varAssign =>
			                                                                                                                                  {
			                                                                                                                                  	var indexEquals = varAssign.IndexOf('=');
																																																																					var varName = varAssign.Substring(0, indexEquals);
																																																																					var varValue = varAssign.Substring(indexEquals + 1);
																																																																					return new KeyValuePair<string, string>(varName, varValue);
			                                                                                                                                  }).ToList();
		}
	}
}