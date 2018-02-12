using System.Collections.Generic;

namespace QDev.QKit.Grouping
{
    public class KeyedListGroup : List<object>, IKeyedGroup
    {
        public KeyedListGroup()
            : this(null, KeyedGroupType.Default)
        { }

        public KeyedListGroup(object key, KeyedGroupType groupType)
            : this(key, key == null ? string.Empty : key.ToString(), groupType)
        { }

        public KeyedListGroup(object key, string keyDisplay, KeyedGroupType groupType)
        {
            Key = key;
            KeyDisplay = keyDisplay;
            GroupType = groupType;
        }

        /// <summary>
        /// Key that represents the identifier of group of objects.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        /// Display value that represents the group and used as the group header.
        /// </summary>
        public string KeyDisplay { get; set; }

        /// <summary>
        /// Type of group used to determine how to display the group header.
        /// </summary>
        public KeyedGroupType GroupType { get; }
    }
}
