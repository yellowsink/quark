using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Quark
{
	/// <summary>
	/// Drop-in replacement for System.Linq.Enumerable optimising for performance where possible using IList
	/// </summary>
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
	[SuppressMessage("ReSharper", "InlineTemporaryVariable")]
	[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
	public static partial class Linq
	{
		/// <summary>
		/// Runs a function over the list, keeping track of a value as it goes - good for cumulative frequencies etc.
		/// </summary>
		/// <param name="source">The list to iterate over</param>
		/// <param name="seed">The seed to start with</param>
		/// <param name="func">This predicate generates the new seed from the old seed and current value</param>
		/// <typeparam name="TIn">The type of the source elements</typeparam>
		/// <typeparam name="TOut">The type of the seed and output</typeparam>
		/// <returns>The fully aggregated seed</returns>
		public static TOut Aggregate<TIn, TOut>(this IList<TIn> source, TOut seed, Func<TOut, TIn, TOut> func)
		{
			for (var i = 0; i < source.Count; i++)
				seed = func(seed, source[i]);
			return seed;
		}

		/// <summary>
		/// Returns true if all elements match a predicate, else false
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>If all elements match the predicate or not</returns>
		public static bool All<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (var i = 0; i < source.Count; i++)
				if (!predicate(source[i]))
					return false;

			return true;
		}
		
		/// <summary>
		/// If the list contains any elements
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>If there are any elements in the list</returns>
		public static bool Any<T>(this IList<T> source) => source.Count != 0;

		/// <summary>
		/// If the list contains any elements that match a predicate
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>If there are any elements in the list matching the predicate</returns>
		public static bool Any<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (var i = 0; i < source.Count; i++)
				if (predicate(source[i]))
					return true;

			return false;
		}

		/// <summary>
		/// Returns a list with the original contents of the list, plus another item
		/// </summary>
		/// <param name="source">A list to start from</param>
		/// <param name="element">The element to add</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>A new list with the element appended</returns>
		public static List<T> Append<T>(this IEnumerable<T> source, T element) => new(source) { element };

		// this seems ultra useless at first glance but it isnt - check docs link
		/// <summary>
		/// Downcasts an IEnumerable implementing class to an IEnumerable
		/// For more info see https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.asenumerable
		/// </summary>
		/// <param name="source">The enumerable to downcast</param>
		/// <typeparam name="T">The type of the enumerable elements</typeparam>
		/// <returns>A downcasted enumerable</returns>
		public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<T> source) => source;

		/// <summary>
		/// Casts a list of elements from TIn to TOut
		/// </summary>
		/// <param name="source">The uncasted list</param>
		/// <typeparam name="TIn">The type of elements in the source</typeparam>
		/// <typeparam name="TOut">The required output type</typeparam>
		/// <returns>A casted list</returns>
		public static List<TOut> Cast<TIn, TOut>(this IList<TIn> source) => source.NonGeneric().Cast<TOut>();
		
		/// <summary>
		/// Casts an untyped list of elements to TOut
		/// </summary>
		/// <param name="source">An untyped list of elements</param>
		/// <typeparam name="T">The target type</typeparam>
		/// <returns>A casted list</returns>
		public static List<T> Cast<T>(this IList source)
		{
			var working = new List<T>();
			for (var i = 0; i < source.Count; i++)
				working.Add((T) source[i]);

			return working;
		}

		/// <summary>
		/// Concatenates two lists together
		/// </summary>
		/// <param name="source">The list to start with</param>
		/// <param name="second">The list to concatenate onto the end</param>
		/// <typeparam name="T">The type of elements in the lists</typeparam>
		/// <returns>The two lists concatenated</returns>
		public static List<T> Concat<T>(this IList<T> source, IList<T> second)
		{
			var tmp = new List<T>(source.Count + second.Count);
			
			for (var i = 0; i < source.Count; i++) tmp[i]                = source[i];
			for (var i = 0; i < second.Count; i++) tmp[i + source.Count] = second[i];

			return tmp;
		}
		
		/// <summary>
		/// Checks if the list contains the given element using a linear search
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <param name="element">The element to look for</param>
		/// <typeparam name="T">The type of the elements</typeparam>
		/// <returns>If the list contains the element</returns>
		public static bool Contains<T>(this IList<T> source, T element)
		{
			for (var i = 0; i < source.Count; i++)
				if (source[i]?.Equals(element) ?? false)
					return true;

			return false;
		}

		/// <summary>
		/// Gets the length of the list
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <typeparam name="T">The type of the elements in the list</typeparam>
		/// <returns>The length of the list</returns>
		public static int Count<T>(this IList<T> source) => source.Count;

		/// <summary>
		/// Counts how many list elements match the predicate
		/// </summary>
		/// <param name="source">The list to count through</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>How many elements matched the predicate</returns>
		public static int Count<T>(this IList<T> source, Predicate<T> predicate)
		{
			switch (source.Count)
			{
				case 0:
					return 0;
				case 1 when predicate(source[1]):
					return 1;
				default:
					var count = 0;
					for (var i = 0; i < source.Count; i++)
						if (predicate(source[i]))
							count++;

					return count;
			}
		}

		/// <summary>
		/// If the list is empty, return a list containing only one instance of the default value of T
		/// </summary>
		/// <param name="source">The list to check against</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The list, or if empty, a list with the default value of T once</returns>
		public static IList<T?> DefaultIfEmpty<T>(this IList<T> source) => source!.DefaultIfEmpty(default);
		
		/// <summary>
		/// If the list is empty, return a list containing defaultValue once
		/// </summary>
		/// <param name="source">The list to check</param>
		/// <param name="defaultValue">The value to fall back on</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The list, or if emtpy, a list with the defaultValue</returns>
		public static IList<T> DefaultIfEmpty<T>(this IList<T> source, T defaultValue)
			=> source.Count == 0 ? new List<T> { defaultValue } : source;

		/// <summary>
		/// Removes duplicates from an enumerable and returns it as an array
		/// </summary>
		/// <param name="source">The enumerable from which to remove duplicates</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The distinct elements in an array</returns>
		public static T[] Distinct<T>(this IEnumerable<T> source)
		{
			var hs  = new HashSet<T>(source);
			var tmp = new T[hs.Count];
			hs.CopyTo(tmp);
			return tmp;
		}

		/// <summary>
		/// Gets the element at the given index
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <param name="index">The index to get</param>
		/// <typeparam name="T">The type of the elements in the list</typeparam>
		/// <returns>The element at the given index</returns>
		public static T ElementAt<T>(this IList<T> source, int index) => source[index];

		/// <summary>
		/// Gets the element at the given index, or default if out of bounds
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <param name="index">The index to try to get</param>
		/// <typeparam name="T">The type of the elements in the list</typeparam>
		/// <returns>The element at the index, or default</returns>
		public static T? ElementAtOrDefault<T>(this IList<T> source, int index)
			=> index < source.Count ? source[index] : default;

		/// <summary>
		/// Performs a set except on source and second
		/// </summary>
		/// <param name="source">The list to begin with</param>
		/// <param name="second">The list to except with source</param>
		/// <typeparam name="T">The type of elements in the lists</typeparam>
		/// <returns>The set except of source and second as an array</returns>
		public static T[] Except<T>(this IList<T> source, IList<T> second)
		{
			var hs = new HashSet<T>(source);
			hs.ExceptWith(second);
			var arr = new T[hs.Count];
			hs.CopyTo(arr);
			return arr;
		}

		/// <summary>
		/// Gets the first item of the list
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <typeparam name="T">The type of items in the list</typeparam>
		/// <returns>The first item in the list</returns>
		/// <exception cref="InvalidOperationException">The list had no elements</exception>
		public static T First<T>(this IList<T> source)
			=> source.Count == 0 ? throw new InvalidOperationException($"{nameof(source)} has no elements") : source[0];

		/// <summary>
		/// Gets the first element of the list that matches the given predicate
		/// </summary>
		/// <param name="source">The list to check through</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>The first element in the list matching the predicate</returns>
		/// <exception cref="InvalidOperationException">The list was empty, or no matches were found</exception>
		public static T First<T>(this IList<T> source, Predicate<T> predicate)
		{
			if (source.Count == 0)
				throw new InvalidOperationException($"{nameof(source)} has no elements");

			for (var i = 0; i < source.Count; i++)
				if (predicate(source[i]))
					return source[i];

			throw new InvalidOperationException($"{nameof(source)} has no matches against {nameof(predicate)}");
		}

		/// <summary>
		/// Gets the first item of the list, or default of T if empty
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The first item of the list or default</returns>
		public static T? FirstOrDefault<T>(this IList<T> source)
			=> source.Count == 0 ? default : source[0];
		
		/// <summary>
		/// Gets the first element of the list that matches the given predicate, or default if none
		/// </summary>
		/// <param name="source">The list to check through</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The first item that matches the predicate or default</returns>
		public static T? FirstOrDefault<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (var i = 0; i < source.Count; i++)
				if (predicate(source[i]))
					return source[i];

			return default;
		}

		/// <summary>
		/// Groups elements together based on keys given by the key selector
		/// </summary>
		/// <param name="source">A list of elements to group</param>
		/// <param name="keySel">A function to get the key</param>
		/// <typeparam name="TIn">The type of the list elements</typeparam>
		/// <typeparam name="TKey">They type of the keys</typeparam>
		/// <returns>An array of groupings of elements by key</returns>
		public static Grouping<TKey, TIn>[] GroupBy<TIn, TKey>(this IList<TIn> source, Func<TIn, TKey> keySel)
			=> source.GroupBy(keySel, a => a);

		/// <summary>
		/// Groups elements together based on keys given by the key selector, then maps through the result selector
		/// </summary>
		/// <param name="source">The list of elements to group</param>
		/// <param name="keySel">A function to get the keys</param>
		/// <param name="resSel">A function to turn a grouping of elements by key into a result element</param>
		/// <typeparam name="TIn">The type of the list elements</typeparam>
		/// <typeparam name="TKey">The type of the keys</typeparam>
		/// <typeparam name="TRes">The type of the result elements</typeparam>
		/// <returns>An array of results</returns>
		public static TRes[] GroupBy<TIn, TKey, TRes>(this IList<TIn>             source, Func<TIn, TKey> keySel,
													  Func<TKey, List<TIn>, TRes> resSel)
			=> source.GroupBy(keySel, a => a, resSel);
		
		/// <summary>
		/// Groups elements together based on keys given by the key selector, mapping the elements through elemSel
		/// </summary>
		/// <param name="source">The list of elements to group</param>
		/// <param name="keySel">A function to get the keys</param>
		/// <param name="elemSel">A function to pass every element through</param>
		/// <typeparam name="TIn">The type of the source elements</typeparam>
		/// <typeparam name="TKey">The type of the keys</typeparam>
		/// <typeparam name="TElem">The type of the result grouping elements</typeparam>
		/// <returns>A list of groupings of element by key</returns>
		public static Grouping<TKey, TElem>[] GroupBy<TIn, TKey, TElem>(this IList<TIn>  source, Func<TIn, TKey> keySel,
																		Func<TIn, TElem> elemSel)
			=> source.GroupBy(keySel, elemSel, (k, el) => new Grouping<TKey, TElem>(k, el));

		/// <summary>
		/// Groups elements together based on keys, mapping elements through elemSel, mapping the groups through resSel
		/// </summary>
		/// <param name="source">The list of elements to group</param>
		/// <param name="keySel">A function to get the keys</param>
		/// <param name="elemSel">A function to pass every element through</param>
		/// <param name="resSel">A function to turn a grouping of elements by key into a result element</param>
		/// <typeparam name="TIn">The type of the source elements</typeparam>
		/// <typeparam name="TKey">The type of the keys</typeparam>
		/// <typeparam name="TElem">The type of the grouping elements</typeparam>
		/// <typeparam name="TRes">The type of the result elements</typeparam>
		/// <returns>An array of results</returns>
		public static TRes[] GroupBy<TIn, TKey, TElem, TRes>(this IList<TIn> source, Func<TIn, TKey> keySel,
															 Func<TIn, TElem> elemSel,
															 Func<TKey, List<TElem>, TRes> resSel)
		{
			var workingTable = new Dictionary<TKey, List<TElem>>();
			for (var i = 0; i < source.Count; i++)
			{
				var key = keySel(source[i]);
				if (!workingTable.ContainsKey(key))
					workingTable[key] = new List<TElem> { elemSel(source[i]) };
				else
				{
					var list = workingTable[key];
					list.Add(elemSel(source[i]));
					workingTable[key] = list;
				}
			}

			var working = new TRes[workingTable.Count];
			var j       = 0;
			foreach (var key in workingTable.Keys)
			{
				working[j] = resSel(key, workingTable[key]);
				j++;
			}

			return working;
		}

		// TODO: GroupJoin()

		/// <summary>
		/// Performs the set intersect on source and second
		/// </summary>
		/// <param name="source">The list to start with</param>
		/// <param name="second">The list to intersect source with</param>
		/// <typeparam name="T">The type of the elements in the lists</typeparam>
		/// <returns>An array of the intersect results</returns>
		public static T[] Intersect<T>(this IList<T> source, IList<T> second)
		{
			var hs = new HashSet<T>(source);
			hs.IntersectWith(second);
			var arr = new T[hs.Count];
			hs.CopyTo(arr);
			return arr;
		}

		/// <summary>
		/// Matches pairs of elements from both lists based on keys
		/// </summary>
		/// <param name="source">The list to attempt to match elements of</param>
		/// <param name="second">The list of possible pairs for elements of source</param>
		/// <param name="sourceKeySel">A function to find keys for source</param>
		/// <param name="secondKeySel">A function to find keys for second</param>
		/// <param name="resSel">A function to find a result element from an element of source and second</param>
		/// <typeparam name="T1">The type of source elements</typeparam>
		/// <typeparam name="T2">The type of second elements</typeparam>
		/// <typeparam name="TK">The type of the keys</typeparam>
		/// <typeparam name="TR">The type of the result elements</typeparam>
		/// <returns>The results of matching pairs</returns>
		public static List<TR> Join<T1, T2, TK, TR>(this IList<T1> source, IList<T2> second, Func<T1, TK> sourceKeySel,
													Func<T2, TK>   secondKeySel, Func<T1, T2, TR> resSel)
		{
			var lookupTable = second.ToDictionary(secondKeySel);
			var working    = new List<TR>();
			
			for (var i = 0; i < source.Count; i++)
			{
				var key = sourceKeySel(source[i]);
				if (lookupTable.ContainsKey(key))
					working.Add(resSel(source[i], lookupTable[key]));
			}

			return working;
		}

		/// <summary>
		/// Gets the last element of the list
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <typeparam name="T">The type of the elements of the list</typeparam>
		/// <returns>The last element of the list</returns>
		/// <exception cref="InvalidOperationException">The list was empty</exception>
		public static T Last<T>(this IList<T> source)
			=> source.Count == 0
				   ? throw new InvalidOperationException($"{nameof(source)} has no elements")
				   : source[source.Count - 1];

		/// <summary>
		/// Gets the last element of the list that matches a predicate
		/// </summary>
		/// <param name="source">The list to search through</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>The last element of the list matching the predicate</returns>
		/// <exception cref="InvalidOperationException">The list was empty, or no matches were found</exception>
		public static T Last<T>(this IList<T> source, Predicate<T> predicate)
		{
			if (source.Count == 0)
				throw new InvalidOperationException($"{nameof(source)} has no elements");
			
			for (var i = source.Count - 1; i >= 0; i--)
				if (predicate(source[i]))
					return source[i];
			
			throw new InvalidOperationException($"{nameof(source)} has no matches against {nameof(predicate)}");
		}

		/// <summary>
		/// Gets the last element of the list, or default if empty
		/// </summary>
		/// <param name="source">The list to get from</param>
		/// <typeparam name="T">The type of the list elements</typeparam>
		/// <returns>The last element of the list or default</returns>
		public static T? LastOrDefault<T>(this IList<T> source)
			=> source.Count == 0 ? default : source[source.Count - 1];
		
		/// <summary>
		/// Gets the last element of the list that matches the predicate, or default
		/// </summary>
		/// <param name="source">The list to search through</param>
		/// <param name="predicate">The predicate to check against</param>
		/// <typeparam name="T">The type of elements in the list</typeparam>
		/// <returns>The last element matching the predicate or default</returns>
		public static T? LastOrDefault<T>(this IList<T> source, Predicate<T> predicate)
		{
			for (var i = source.Count - 1; i >= 0; i--)
				if (predicate(source[i]))
					return source[i];

			return default;
		}

		// TODO: keep writing docs from here!
		
		public static long LongCount<T>(this IList<T> source)
			=> throw new NotImplementedException($"Quark does not implement {nameof(LongCount)}");

		public static long LongCount<T>(this IList<T> source, Predicate<T> predicate)
			=> throw new NotImplementedException($"Quark does not implement {nameof(LongCount)}");

		public static IList NonGeneric<T>(this IList<T> source) => (IList) source;
		
		public static List<T> OfType<T>(this IList source)
		{
			var working = new List<T>(source.Count);
			for (var i = 0; i < source.Count; i++)
				if (source[i] is T typed)
					working.Add(typed);

			return working;
		}

		public static TElem[] OrderBy<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector)
			=> source.OrderBy(selector, Comparer<TKey>.Default);
		
		public static TElem[] OrderBy<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector, IComparer<TKey> comparer)
		{
			var arr = new TElem[source.Count];
			for (var i = 0; i < source.Count; i++) 
				arr[i] = source[i];
			
			QuickSort<TElem, TKey>.SortInPlace(ref arr, selector, comparer);
			
			return arr;
		}
		
		public static TElem[] OrderByDescending<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector)
			=> source.OrderByDescending(selector, Comparer<TKey>.Default);

		public static TElem[] OrderByDescending<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector, IComparer<TKey> comparer)
		{
			var arr = new TElem[source.Count];
			for (var i = 0; i < source.Count; i++) 
				arr[i] = source[i];
			
			QuickSort<TElem, TKey>.SortInPlace(ref arr, selector, comparer, true);
			
			return arr;
		}

		public static List<T> Prepend<T>(this IList<T> source, T elem)
		{
			var tmp = new List<T>(source.Count + 1) { [0] = elem };
			for (var i = 0; i < source.Count; i++)
				tmp[i + 1] = source[i];
			
			return tmp;
		}

		public static int[] Range(int start, int count)
		{
			var working = new int[count];
			for (var i = 0; i < count; i++)
				working[i] = i + start;
			
			return working;
		}

		public static T[] Repeat<T>(T element, int count)
		{
			var working = new T[count];
			for (var i = 0; i < count; i++) 
				working[i] = element;

			return working;
		}

		public static List<T> Reverse<T>(this IList<T> source)
		{
			var tmp = new List<T>(source);
			for (var i = 0; i < tmp.Count / 2; i++)
				(tmp[i], tmp[tmp.Count - i]) = (tmp[tmp.Count - i], tmp[i]);

			return tmp;
		}

		public static TOut[] Select<TIn, TOut>(this IList<TIn> source, Func<TIn, TOut> func)
			=> source.Select((a, _) => func(a));
		
		public static TOut[] Select<TIn, TOut>(this IList<TIn> source, Func<TIn, int, TOut> func)
		{
			var working = new TOut[source.Count];
			for (var i = 0; i < source.Count; i++) 
				working[i] = func(source[i], i);

			return working;
		}

		public static List<T> SelectMany<T>(this IList<IList<T>> source)
			// discard the index parameter here even tho its not necessary to reduce nesting of delegates
			=> source.SelectMany((a, _) => a);
		
		public static List<TOut> SelectMany<TIn, TOut>(this IList<TIn> source, Func<TIn, IList<TOut>> func)
			=> source.SelectMany((a, _) => func(a));

		public static List<TOut> SelectMany<TIn, TOut>(this IList<TIn> source, Func<TIn, int, IList<TOut>> func)
		{
			// the final size will almost certainly exceed the count, but its a good baseline
			var working = new List<TOut>(source.Count);
			for (var i = 0; i < source.Count; i++)
			{
				var sublist = func(source[i], i);
				for (var j = 0; j < sublist.Count; j++)
					working.Add(sublist[j]);
			}

			return working;
		}

		public static bool SequenceEqual<T1, T2>(this IList<T1> source, IList<T2> second)
		{
			if (source.Count != second.Count) return false;
			if (second is not IList<T1> secondTyped) return false;

			for (var i = 0; i < source.Count; i++)
				if (!(source[i]?.Equals(secondTyped[i]) ?? false))
					return false;

			return true;
		}

		public static T Single<T>(this IList<T> source)
			=> source.Count switch
			{
				0 => throw new InvalidOperationException($"{nameof(source)} has no elements"),
				1 => source[0],
				_ => throw new InvalidOperationException($"{nameof(source)} has more than one element")
			};

		public static T Single<T>(this IList<T> source, Predicate<T> predicate)
		{
			if (source.Count == 0) throw new InvalidOperationException($"{nameof(source)} has no elements");

			int? match = null;
			for (var i = 0; i < source.Count; i++)
			{
				if (!predicate(source[i])) continue;
				
				if (match.HasValue)
					throw new
						InvalidOperationException($"{nameof(source)} has multiple matches against {nameof(predicate)}");
				match = i;
			}

			if (match.HasValue)
				return source[match.Value];
			
			throw new InvalidOperationException($"{nameof(source)} has no matches against {nameof(predicate)}");
		}

		public static T? SingleOrDefault<T>(this IList<T> source) => source.Count == 1 ? source[0] : default;

		public static T? SingleOrDefault<T>(this IList<T> source, Predicate<T> predicate)
		{
			if (source.Count == 0) return default;

			int? match = null;
			for (var i = 0; i < source.Count; i++)
			{
				if (!predicate(source[i])) continue;

				if (match.HasValue) return default;
				
				match = i;
			}

			return match.HasValue ? source[match.Value] : default;
		}

		public static List<T> Skip<T>(this IList<T> source, int count)
		{
			var working = new List<T>(source.Count - count);
			for (var i = 0; i < source.Count - count; i++)
				working[i] = source[i + count];

			return working;
		}
		
		public static List<T> SkipLast<T>(this IList<T> source, int count)
		{
			var working = new List<T>(source.Count - count);
			for (var i = 0; i < source.Count - count; i++)
				working[i] = source[i];

			return working;
		}

		public static List<T> SkipWhile<T>(this IList<T> source, Predicate<T> predicate)
		{
			var working = new List<T>();
			for (var i = 0; i < source.Count; i++)
			
				if (!predicate(source[i]))
					working.Add(source[i]);

			return working;
		}

		public static List<T> Take<T>(this IList<T> source, int count)
		{
			var working = new List<T>(count);
			for (var i = 0; i < count; i++)
				working[i] = source[i];

			return working;
		}

		public static List<T> TakeLast<T>(this IList<T> source, int count)
		{
			var working = new List<T>(count);
			for (var i = 0; i < count; i++)
				working[i] = source[i + (source.Count - count)];

			return working;
		}
		
		public static List<T> TakeWhile<T>(this IList<T> source, Predicate<T> predicate)
		{
			var working = new List<T>();
			for (var i = 0; i < source.Count; i++)
			
				if (predicate(source[i]))
					working.Add(source[i]);

			return working;
		}

		public static TElem[] ThenBy<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector)
			=> source.ThenBy(selector, Comparer<TKey>.Default);


		private static TElem[] ThenBy<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector,
												   IComparer<TKey>   comparer)
			=> source.ThenBy(selector, comparer, false);
		
		public static TElem[] ThenByDescending<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector)
			=> source.ThenByDescending(selector, Comparer<TKey>.Default);

		public static TElem[] ThenByDescending<TElem, TKey>(this IList<TElem> source, Func<TElem, TKey> selector,
															IComparer<TKey>   comparer)
			=> source.ThenBy(selector, comparer, true);

		private static TElem[] ThenBy<TElem, TKey>(this IList<TElem> source,   Func<TElem, TKey> selector,
												   IComparer<TKey>   comparer, bool              reverse)
		{
			var arr = new TElem[source.Count];
			for (var i = 0; i < source.Count; i++)
				arr[i] = source[i];

			var lo = 0;
			var hi = 0;
			for (var i = 1; i < source.Count; i++)
			{
				if (source[i]?.Equals(source[lo]) ?? false)
					hi = i;
				else
				{
					QuickSort<TElem, TKey>.SortInPlace(ref arr, selector, comparer, reverse, lo, hi);
					lo = i;
				}
			}

			return arr;
		}

		public static T[] ToArray<T>(this IList<T> source)
		{
			switch (source)
			{
				case T[] arr:
					return arr;
				default:
					var working = new T[source.Count];
					for (var i = 0; i < source.Count; i++) 
						working[i] = source[i];

					return working;
			}
		}

		public static Dictionary<TKey, TElem> ToDictionary<TElem, TKey>(
			this IList<TElem> source, Func<TElem, TKey> keySel)
			=> source.ToDictionary(keySel, a => a);

		public static Dictionary<TKey, TElem> ToDictionary<TIn, TKey, TElem>(
			this IList<TIn> source, Func<TIn, TKey> keySel, Func<TIn, TElem> elemSel)
		{
			var working = new Dictionary<TKey, TElem>();
			for (var i = 0; i < source.Count; i++)
				working.Add(keySel(source[i]), elemSel(source[i]));

			return working;
		}

		public static HashSet<T> ToHashSet<T>(this IList<T> source) => new(source);

		public static List<T> ToList<T>(this IEnumerable<T> source)
		{
			switch (source)
			{
				case List<T> list:
					return list;
				case IReadOnlyList<T> irol:
					var workingl = new List<T>(irol.Count);
					for (var i = 0; i < irol.Count; i++)
						workingl[i] = irol[i];

					return workingl;
				default:
				{
					var       workinge   = new List<T>();
					using var enumerator = source.GetEnumerator();
					while (enumerator.MoveNext())
						workinge.Add(enumerator.Current);
					return workinge;
				}
			}
		}

		public static Lookup<TKey, TElem> ToLookup<TIn, TKey, TElem>(this IList<TIn>  source, Func<TIn, TKey> keySel,
																	 Func<TIn, TElem> elemSel)
			=> Lookup<TKey, TElem>.Create(source, keySel, elemSel);
		
		public static T[] Union<T>(this IList<T> source, IList<T> second)
		{
			var hs  = new HashSet<T>(source);
			hs.UnionWith(second);
			var arr = new T[hs.Count];
			hs.CopyTo(arr);
			return arr;
		}

		public static List<T> Where<T>(this IList<T> source, Predicate<T> predicate)
			=> source.Where((a, _) => predicate(a));

		public static List<T> Where<T>(this IList<T> source, Func<T, int, bool> predicate)
		{
			var working = new List<T>();
			for (var i = 0; i < source.Count; i++)
				if (predicate(source[i], i))
					working.Add(source[i]);

			return working;
		}

		public static (T1, T2)[] Zip<T1, T2, TRes>(this IList<T1> source, IList<T2> second)
			=> source.Zip(second, (a, b) => (a, b));
		
		public static TRes[] Zip<T1, T2, TRes>(this IList<T1> source, IList<T2> second, Func<T1, T2, TRes> resSel)
		{
			var working = new TRes[Math.Min(source.Count, second.Count)];
			for (var i = 0; i < working.Length; i++)
				working[i] = resSel(source[i], second[i]);

			return working;
		}
	}
}