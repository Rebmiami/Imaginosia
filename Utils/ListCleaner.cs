using System;
using System.Collections.Generic;
using System.Text;

namespace Imaginosia
{
	public static class ListCleaner //Removes objects from lists using custom filters because foreach is an asshole.
	{
		public delegate bool Filter<T>(T obj); //Return true to remove the item

		// Summary:
		//     Removes elements from a list using a delegate filter
		//     Return "false" to keep an element, and return "true" to remove it
		public static void CleanList<T>(List<T> list, Filter<T> filter)
        {
			List<T> toRemove = new List<T>();
			foreach (T obj in list)
            {
				if (filter(obj))
                {
					toRemove.Add(obj);
                }
            }
			foreach (T obj in toRemove)
            {
				list.Remove(obj);
            }
        }
	}
}