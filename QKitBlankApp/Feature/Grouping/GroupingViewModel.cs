using QKit.Grouping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace QKitBlankApp.Feature.Grouping
{
    public class GroupingViewModel
    {
        public GroupingViewModel()
        {
            var items = new List<string>
            {
                "burnley",
                "cohobating",
                "tentorial",
                "densus",
                "block",
                "signorina",
                "reinquiring",
                "aphacia",
                "untrammelled",
                "rite",
                "unmarled",
                "laicize",
                "liest",
                "vinylethylene",
                "tuba",
                "prepromoting",
                "parodontia",
                "symbiotically",
                "predominate",
                "nonlaminated",
                "nonobstructive",
                "nonmicroscopic",
                "diu",
                "smokier",
                "juniper",
                "ptotic",
                "vain",
                "undone",
                "overpersuasion",
                "nonaffiliation",
                "unground",
                "hoggin",
                "norite",
                "thievingly",
                "cleanliness",
                "unwaned",
                "preissue",
                "molokai",
                "kowtow",
                "nonmetrical",
                "spuriousness",
                "laughingly",
                "unshed",
                "mummied",
                "noncombat",
                "horrify",
                "diphenhydramine",
                "houselling",
                "gaudeamus",
                "vouchsafement"
            };
            
            var groupedItems = items.ToAlphaGroups(x => x);

            ViewSource = new CollectionViewSource { Source = groupedItems, IsSourceGrouped = true };
        }

        public CollectionViewSource ViewSource { get; set; }
    }
}
