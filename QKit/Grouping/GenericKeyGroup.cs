namespace QKit.Grouping
{
    public class GenericKeyGroup : KeyedCollectionGroup
    {
        /// <summary>
        /// Display a string value that represents the group and used as the group header.
        /// </summary>
        public string KeyDisplay { get; set; }

        public override KeyedGroupType GroupType
        {
            get { return KeyedGroupType.Default; }
        }
    }
}
