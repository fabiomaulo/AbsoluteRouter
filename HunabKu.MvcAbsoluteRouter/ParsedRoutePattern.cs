using System;
using System.Linq;

namespace HunabKu.MvcAbsoluteRouter
{
	public class ParsedRoutePattern
	{
		private static readonly string[] SchemeAtStart = {
		                                                 	Uri.UriSchemeHttp + Uri.SchemeDelimiter,
		                                                 	Uri.UriSchemeHttps + Uri.SchemeDelimiter
		                                                 };

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

		public string LocalPattern { get; private set; }

		public string DnsSafeHostPattern { get; private set; }

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}

		private void ExtractPatterns(string urlPattern)
		{
			string containedScheme = SchemeAtStart.FirstOrDefault(x => urlPattern.StartsWith(x));
			string urlCleanedFromScheme = urlPattern;
			if (containedScheme != null)
			{
				urlCleanedFromScheme = urlPattern.Substring(containedScheme.Length);
			}
			int indexOfFirstSlash = urlCleanedFromScheme.IndexOf('/');
			bool hasPath = indexOfFirstSlash >= 0;
			bool hasDns = urlCleanedFromScheme.IndexOf('.') >= 0;

			DnsSafeHostPattern = hasPath ? urlCleanedFromScheme.Substring(0, indexOfFirstSlash) : (hasDns ? urlCleanedFromScheme : string.Empty);

			string pathAndQuery = hasPath ? urlCleanedFromScheme.Substring(indexOfFirstSlash + 1) : (hasDns ? string.Empty : urlCleanedFromScheme);
			int indexOfQueryStringStart = pathAndQuery.IndexOf('?');
			LocalPattern = indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
		}
	}
}