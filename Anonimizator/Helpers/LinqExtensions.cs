using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonimizator.Helpers
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> TakeWhileAggregate<TSource, TAccumulate>(
            this IEnumerable<TSource> source,
            TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func,
            Func<TAccumulate, bool> predicate
        )
        {
            TAccumulate accumulator = seed;
            foreach (TSource item in source)
            {
                accumulator = func(accumulator, item);
                if (predicate(accumulator))
                {
                    yield return item;
                }
                else
                {
                    yield return item;
                    yield break;
                }
            }
        }
    }
}
