using System;
using System.Collections.Generic;
using System.Linq;

namespace Functional
{
    public static class IEnumerableExtensions
    {
                                          
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector) 
        {
            return outer.GroupJoin(inner,
                outerKeySelector, 
                innerKeySelector, 
                (outerItem, innerGroup) => new { 
                    Outer = outerItem, 
                    InnerGroup = innerGroup 
                })
                .SelectMany(group => group.InnerGroup.DefaultIfEmpty(), 
                    (group, innerGroup) => resultSelector(group.Outer, innerGroup));
        }
        
        public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> firstSequence,
            IEnumerable<TSecond> secondSequence,
            Func<TFirst, TSecond, TResult> resultSelector) 
        {
            return firstSequence.SelectMany(first => 
                    secondSequence.Select(second => resultSelector(first, second)));
        }
        
        public static void Each<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach(T item in enumeration)
            {
                action(item);
            }
        }
        
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> enumeration)
        {
            while (true) {
                foreach (T item in enumeration)
                {
                    yield return item;
                }
            }
        }
        
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize) 
        { 
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext()) 
                {
                    yield return YieldBatchElements(enumerator, batchSize - 1);
                }
            }
        } 

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source,  int batchSize) 
        { 
            yield return source.Current;
            
            for (int i = 0; i < batchSize && source.MoveNext(); i++) 
            {
                yield return source.Current; 
            }
        } 
        
        public static IEnumerable<IEnumerable<TSource>> Break<TSource>(this IEnumerable<TSource> source,
                    Predicate<TSource> predicate)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return BreakWhere(enumerator, predicate);
                }
            }
        }
        
        private static IEnumerable<T> BreakWhere<T>(IEnumerator<T> enumerator,
                          Predicate<T> predicate)
        {
            do
            {
                yield return enumerator.Current;
            } while (!predicate(enumerator.Current) && enumerator.MoveNext());
        }
    }
}