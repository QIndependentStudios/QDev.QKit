namespace QKit.Uwp.Grouping
{
    public interface IKeyedGroup
    {
        object Key { get; set; }
        string KeyDisplay { get; set; }
        KeyedGroupType GroupType { get; }
    }
}
