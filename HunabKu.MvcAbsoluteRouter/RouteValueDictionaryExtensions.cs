using System;
using System.Linq;
using System.Web.Routing;

namespace HunabKu.MvcAbsoluteRouter
{
	internal static class RouteValueDictionaryExtensions
	{
		public static void OverrideMergeWith(this RouteValueDictionary destination, RouteValueDictionary source)
		{
			if (source == null)
			{
				return;
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			foreach (var value in source)
			{
				destination[value.Key] = value.Value;
			}
		}

		public static void MergeWith(this RouteValueDictionary destination, RouteValueDictionary source)
		{
			if (source == null)
			{
				return;
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			foreach (var value in source.Where(x => !destination.ContainsKey(x.Key)))
			{
				destination[value.Key] = value.Value;
			}
		}
	}
}