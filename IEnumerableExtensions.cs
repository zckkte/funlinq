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
    }
}