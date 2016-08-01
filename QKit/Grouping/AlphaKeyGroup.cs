namespace QKit.Grouping
{
    public class AlphaKeyGroup : KeyedCollectionGroup
    {
        /// <summary>
        /// Display character that represents the group and used as the group header.
        /// </summary>
        public string KeyDisplay { get; set; }

        public override KeyedGroupType GroupType
        {
            get { return KeyedGroupType.Alpha; }
        }
    }
}
