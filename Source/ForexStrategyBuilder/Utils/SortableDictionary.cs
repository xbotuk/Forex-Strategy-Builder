//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Collections.Generic;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Sortable dictionary by Keys
    /// </summary>
    public class SortableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : IComparable
    {
        public void Sort()
        {
            // Copy the dictionary data to a List
            var sortedList = new List<KeyValuePair<TKey, TValue>>(this);

            // Use the List's Sort method, and make sure we are comparing Keys.
            sortedList.Sort((first, second) => first.Key.CompareTo(second.Key));

            // Clear the dictionary and repopulate it from the List
            Clear();
            foreach (var kvp in sortedList)
                Add(kvp.Key, kvp.Value);
        }

        public void ReverseSort()
        {
            // Copy the dictionary data to a List
            var sortedList = new List<KeyValuePair<TKey, TValue>>(this);

            // Use the List's Sort method, and make sure we are comparing Keys.
            sortedList.Sort((first, second) => second.Key.CompareTo(first.Key));

            // Clear the dictionary and repopulate it from the List
            Clear();
            foreach (var kvp in sortedList)
                Add(kvp.Key, kvp.Value);
        }
    }
}