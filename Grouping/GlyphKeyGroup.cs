namespace QKit.Grouping
{
    public class GlyphKeyGroup : KeyedCollectionGroup
    {
        /// <summary>
        /// Display glyph that represents the group and used as the group header.
        /// </summary>
        public string Glyph { get; set; }

        public override KeyedGroupType GroupType
        {
            get { return KeyedGroupType.Glyph; }
        }
    }
}
