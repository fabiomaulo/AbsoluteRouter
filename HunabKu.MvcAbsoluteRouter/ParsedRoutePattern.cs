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

			if(string.IsNullOrWhiteSpace(pattern))
			{
				throw new ArgumentOutOfRangeException("pattern","The pattern is empty.");
			}
		}

		public static ParsedRoutePattern Parse(string pattern)
		{
			return new ParsedRoutePattern(pattern);
		}
	}
}