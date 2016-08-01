using System.Collections.Generic;

namespace QKit.Grouping
{
    public abstract class KeyedCollectionGroup : List<object>, IKeyedGroup
    {
        /// <summary>
        /// Key that represents the identifier of group of objects.
        /// </summary>
        public object Key { get; set; }

        public abstract KeyedGroupType GroupType { get; }
    }
}
