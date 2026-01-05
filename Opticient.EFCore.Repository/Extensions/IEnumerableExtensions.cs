using System;
using System.Collections.Generic;
using System.Linq;

namespace Opticient.EFCore.Repository.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}" /> to enhance functionality for collections.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Determines whether the specified collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="self">The collection to check.</param>
    /// <returns><c>true</c> if the collection is null or has no elements; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> self) => self == null || !self.Any();

    /// <summary>
    /// Executes a specified action on each element of the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="self">The collection on which to perform the action.</param>
    /// <param name="action">The action to perform on each element.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!self.IsNullOrEmpty())
        {
            foreach (T t in self)
            {
                action(t);
            }
        }
    }

    /// <summary>
    /// Projects each element of the collection into a new form along with its index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="self">The collection to process.</param>
    /// <returns>An enumerable collection of tuples, each containing an item and its index.</returns>
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
        => self.Select((item, index) => (item, index));

    /// <summary>
    /// Performs a full outer join on two collections based on a specified key selector.
    /// </summary>
    /// <typeparam name="TLeft">The type of elements in the left collection.</typeparam>
    /// <typeparam name="TRight">The type of elements in the right collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for joining.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="leftItems">The left collection.</param>
    /// <param name="rightItems">The right collection.</param>
    /// <param name="leftKeySelector">Function to select keys from the left collection.</param>
    /// <param name="rightKeySelector">Function to select keys from the right collection.</param>
    /// <param name="resultSelector">Function to create the result from left and right elements.</param>
    /// <returns>An enumerable collection of joined results.</returns>
    public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> leftItems,
        IEnumerable<TRight> rightItems,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TResult> resultSelector)
    {
        var leftLookup = leftItems.ToLookup(leftKeySelector);
        var rightLookup = rightItems.ToLookup(rightKeySelector);
        var keys = new HashSet<TKey>(leftLookup.Select(o => o.Key).Concat(rightLookup.Select(i => i.Key)));

        foreach (var key in keys)
        {
            foreach (var leftItem in leftLookup[key].DefaultIfEmpty())
            {
                foreach (var rightItem in rightLookup[key].DefaultIfEmpty())
                {
                    yield return resultSelector(leftItem, rightItem);
                }
            }
        }
    }

    /// <summary>
    /// Performs a left outer join on two collections based on a specified key selector.
    /// </summary>
    /// <typeparam name="TLeft">The type of elements in the left collection.</typeparam>
    /// <typeparam name="TRight">The type of elements in the right collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for joining.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="leftItems">The left collection.</param>
    /// <param name="rightItems">The right collection.</param>
    /// <param name="leftKeySelector">Function to select keys from the left collection.</param>
    /// <param name="rightKeySelector">Function to select keys from the right collection.</param>
    /// <param name="resultSelector">Function to create the result from left and right elements.</param>
    /// <returns>An enumerable collection of joined results.</returns>
    public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> leftItems,
        IEnumerable<TRight> rightItems,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TResult> resultSelector)
    {
        return from left in leftItems
               join right in rightItems on leftKeySelector(left) equals rightKeySelector(right) into joined
               from right in joined.DefaultIfEmpty()
               select resultSelector(left, right);
    }

    /// <summary>
    /// Performs a right outer join on two collections based on a specified key selector.
    /// </summary>
    /// <typeparam name="TLeft">The type of elements in the left collection.</typeparam>
    /// <typeparam name="TRight">The type of elements in the right collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used for joining.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="leftItems">The left collection.</param>
    /// <param name="rightItems">The right collection.</param>
    /// <param name="leftKeySelector">Function to select keys from the left collection.</param>
    /// <param name="rightKeySelector">Function to select keys from the right collection.</param>
    /// <param name="resultSelector">Function to create the result from left and right elements.</param>
    /// <returns>An enumerable collection of joined results.</returns>
    public static IEnumerable<TResult> RightOuterJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> leftItems,
        IEnumerable<TRight> rightItems,
        Func<TLeft, TKey> leftKeySelector,
        Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TResult> resultSelector)
    {
        return from right in rightItems
               join left in leftItems on rightKeySelector(right) equals leftKeySelector(left) into joined
               from left in joined.DefaultIfEmpty()
               select resultSelector(left, right);
    }
}
