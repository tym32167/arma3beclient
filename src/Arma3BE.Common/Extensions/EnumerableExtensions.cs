using System.Collections.Generic;
using System.Linq;

namespace Arma3BEClient.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Paged<T>(this IEnumerable<T> list, int pageSize)
        {
            var result = new List<T[]>();

            var page = new List<T>();

            foreach (var item in list)
            {
                page.Add(item);
                if (page.Count == pageSize)
                {
                    result.Add(page.ToArray());
                    page = new List<T>();
                }
            }

            if (page.Any())
            {
                result.Add(page.ToArray());
            }

            return result;
        }
    }
}