using System;

namespace HunabKu.MvcAbsoluteRouter
{
	public class ParsedRoutePattern
	{
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
		}

		public string OriginalPattern { get; private set; }
		public string SchemePattern { get; private set; }
		public string HostPattern { get; private set; }
		public string LocalPattern { get; private set; }
		public string QueryPattern { get; private set; }

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}

		private void ExtractPatterns(string urlPattern)
		{
			int indexOfSchemeDelimiter = urlPattern.IndexOf(Uri.SchemeDelimiter);
			SchemePattern = string.Empty;
			string urlCleanedFromScheme = urlPattern;
			if (indexOfSchemeDelimiter >= 0)
			{
				SchemePattern = urlPattern.Substring(0, indexOfSchemeDelimiter);
				urlCleanedFromScheme = urlPattern.Substring(indexOfSchemeDelimiter + Uri.SchemeDelimiter.Length);
			}
			int indexOfFirstSlash = urlCleanedFromScheme.IndexOf('/');
			bool hasPath = indexOfFirstSlash >= 0;
			bool hasDns = urlCleanedFromScheme.IndexOf('.') >= 0;

			HostPattern = hasPath ? urlCleanedFromScheme.Substring(0, indexOfFirstSlash) : (hasDns ? urlCleanedFromScheme : string.Empty);

			string pathAndQuery = hasPath ? urlCleanedFromScheme.Substring(indexOfFirstSlash + 1) : (hasDns ? string.Empty : urlCleanedFromScheme);
			int indexOfQueryStringStart = pathAndQuery.IndexOf('?');
			LocalPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
			QueryPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(indexOfQueryStringStart + 1) : string.Empty;
		}
	}
}