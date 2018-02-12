using QDev.QKit.Grouping;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace QDev.QKit.Selectors
{
    public class GroupDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate AlphaGroupDataTemplate { get; set; }
        public DataTemplate GlyphGroupDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            IKeyedGroup keyedGroup;

            keyedGroup = item as IKeyedGroup;
            if (keyedGroup != null)
                return SelectTemplateForKeyedGroup(keyedGroup);

            var collectionViewGroup = item as ICollectionViewGroup;
            if (collectionViewGroup != null)
            {
                keyedGroup = collectionViewGroup.Group as IKeyedGroup;
                if (keyedGroup != null)
                    return SelectTemplateForKeyedGroup(keyedGroup);
                return DefaultDataTemplate;
            }

            return DefaultDataTemplate;
        }

        private DataTemplate SelectTemplateForKeyedGroup(IKeyedGroup keyedGroup)
        {
            switch (keyedGroup.GroupType)
            {
                case KeyedGroupType.Default:
                    return DefaultDataTemplate;
                case KeyedGroupType.Alpha:
                    return AlphaGroupDataTemplate;
                case KeyedGroupType.Glyph:
                    return GlyphGroupDataTemplate;
                default:
                    return DefaultDataTemplate;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplate(item);
        }
    }
}
