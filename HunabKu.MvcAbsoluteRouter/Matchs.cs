using System;

namespace HunabKu.MvcAbsoluteRouter
{
	public class Matchs
	{
		private readonly Func<string, bool> matcher;

		private Matchs(Func<string, bool> matcher)
		{
			if (matcher == null)
			{
				throw new ArgumentNullException("matcher");
			}
			this.matcher = matcher;
		}

		public static Matchs When(Func<string, bool> matcher)
		{
			return new Matchs(matcher);
		}

		public bool Match(string value)
		{
			return matcher(value);
		}
	}
}