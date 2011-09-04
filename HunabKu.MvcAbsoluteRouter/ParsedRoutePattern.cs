using System;
using System.Linq;

namespace HunabKu.MvcAbsoluteRouter
{
	public class ParsedRoutePattern
	{
		private static readonly string[] SchemaAtStart = {
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
			LocalPattern = ExtractLocalPattern(pattern);
		}

		public string OriginalPattern { get; private set; }

		public string LocalPattern { get; private set; }

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}

		private string ExtractLocalPattern(string urlPattern)
		{
			string containedSchema = SchemaAtStart.FirstOrDefault(x => urlPattern.StartsWith(x));
			string urlCleanedFromSchema = urlPattern;
			if (containedSchema != null)
			{
				urlCleanedFromSchema = urlPattern.Substring(containedSchema.Length);
			}
			int indexOfFirstSlash = urlCleanedFromSchema.IndexOf('/');
			bool hasPath = indexOfFirstSlash >= 0;
			bool hasDns = urlCleanedFromSchema.IndexOf('.') >= 0;
			string pathAndQuery = hasPath ? urlCleanedFromSchema.Substring(indexOfFirstSlash + 1) : (hasDns ? string.Empty : urlCleanedFromSchema);
			int indexOfQueryStringStart = pathAndQuery.IndexOf('?');
			return indexOfQueryStringStart > 0 ? pathAndQuery.Substring(0, indexOfQueryStringStart) : pathAndQuery;
		}
	}
}