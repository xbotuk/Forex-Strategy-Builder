// Sortable Dictionary
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Sortable dictionary by Keys
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
            foreach (KeyValuePair<TKey, TValue> kvp in sortedList)
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
            foreach (KeyValuePair<TKey, TValue> kvp in sortedList)
                Add(kvp.Key, kvp.Value);
        }
    }
}
