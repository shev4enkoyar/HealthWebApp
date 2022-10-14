using System.Collections.Generic;
using System.Linq;

namespace HealthWebApp.Extensions
{
    public static class LINQExtensions
    {
        public static IEnumerable<T> Pagination<T>(this IEnumerable<T> value, int page, int pageSize)
        {
            return value.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
