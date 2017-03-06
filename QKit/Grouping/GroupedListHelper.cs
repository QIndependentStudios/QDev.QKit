using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization.Collation;

namespace QKit.Grouping
{
    /// <summary>
    /// Provides a utility to help group and sort data into JumpList compatible data.
    /// </summary>
    public static class GroupedListHelper
    {
        private const string OtherKey = "...";

        /// <summary>
        /// Groups and sorts into a list of group lists based on a selector.
        /// </summary>
        /// <typeparam name="TSource">Type of the items in the list.</typeparam>
        /// <typeparam name="TSort">Type of value returned by sortSelector.</typeparam>
        /// <typeparam name="TGroup">Type of value returned by groupSelector.</typeparam>
        /// <param name="source">List to be grouped and sorted</param>
        /// <param name="sortSelector">A selector that provides the value that items will be sorted by.</param>
        /// <param name="groupSelector">A selector that provides the value that items will be grouped by.</param>
        /// <param name="groupDisplaySelector">A selector that will provide the value represent a group for display.</param>
        /// <returns>A list of JumpListGroups.</returns>
        public static List<KeyedListGroup> ToGroups<TSource, TSort, TGroup>(
            this IEnumerable<TSource> source, Func<TSource, TSort> sortSelector,
            Func<TSource, TGroup> groupSelector, Func<TGroup, string> groupDisplaySelector = null)
        {
            var groups = new List<KeyedListGroup>();

            // Group and sort items based on values returned from the selectors
            var query = from item in source
                        orderby groupSelector(item), sortSelector(item)
                        group item by groupSelector(item) into g
                        select new { GroupName = g.Key, Items = g };

            // For each group generated from the query, create a JumpListGroup
            // and fill it with its items
            foreach (var g in query)
            {
                var group = new KeyedListGroup(g.GroupName,
                    groupDisplaySelector == null
                        ? g.GroupName.ToString()
                        : groupDisplaySelector(g.GroupName),
                    KeyedGroupType.Default);

                foreach (var item in g.Items)
                    group.Add(item);

                groups.Add(group);
            }

            return groups;
        }

        /// <summary>
        /// Groups and sorts into a list of alpha groups based on a string selector.
        /// </summary>
        /// <typeparam name="TSource">Type of the items in the list.</typeparam>
        /// <param name="source">List to be grouped and sorted.</param>
        /// <param name="selector">A selector that will provide a value that items to be sorted and grouped by.</param>
        /// <returns>A list of JumpListGroups.</returns>
        public static List<KeyedListGroup> ToAlphaGroups<TSource>(
            this IEnumerable<TSource> source, Func<TSource, string> selector)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var group = new KeyedListGroup(OtherKey, "", KeyedGroupType.Glyph);

                foreach (var item in source.OrderBy(selector))
                    group.Add(item);
                return new List<KeyedListGroup> { group };
            }

            // Get the letters representing each group for current language using CharacterGroupings class
            var characterGroupings = new CharacterGroupings();

            // Create groups for each letters
            var groupDictionary = new Dictionary<string, KeyedListGroup>();

            foreach (var characterGrouping in characterGroupings)
            {
                var key = string.IsNullOrEmpty(characterGrouping.Label) ? OtherKey : characterGrouping.Label;

                if (groupDictionary.ContainsKey(key))
                    continue;

                if (key == OtherKey && !groupDictionary.ContainsKey(OtherKey))
                {
                    groupDictionary.Add(OtherKey,
                        new KeyedListGroup(OtherKey,
                            "",
                            KeyedGroupType.Glyph));
                }
                else
                {
                    groupDictionary.Add(characterGrouping.Label,
                        new KeyedListGroup(characterGrouping.Label,
                            characterGrouping.Label,
                            KeyedGroupType.Alpha));
                }
            }

            // Sort and group items into the groups based on the value returned by the selector
            var query = from item in source
                        orderby selector(item)
                        select item;

            foreach (var item in query)
            {
                var sortValue = selector(item);
                var label = characterGroupings.Lookup(sortValue);
                var key = groupDictionary.ContainsKey(label) ? label : OtherKey;
                groupDictionary[key].Add(item);
            }

            return groupDictionary.Select(x => x.Value).ToList();
        }
    }
}