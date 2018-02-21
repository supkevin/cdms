using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CDMS.Service.Common
{
    //http://kevintsengtw.blogspot.tw/2014/04/linq-multi-columns-dynamic-group.html
    public class GroupResult {
        public object Key { get; set; }
        public int Count { get; set; }
        public IEnumerable Items { get; set; }
        public IEnumerable<GroupResult> SubGroups { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", Key, Count);
        }
    }

    public static class MyEnumerableExtensions
    {
        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IQueryable<TElement> elements,
            params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();

                //reduce the list recursively until zero
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                var result = elements
                    .GroupBy(selector)
                    .Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });

                return result;
            }
            else
            {
                return null;
            }
        }


        public static IEnumerable<GroupResult> GroupByMany<TElement>(
            this IEnumerable<TElement> elements,
            params Func<TElement, object>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();

                //reduce the list recursively until zero
                var nextSelectors = groupSelectors.Skip(1).ToArray();
                var result = elements
                    .GroupBy(selector)
                    .Select(
                        g => new GroupResult
                        {
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
