using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Museum.Models
{
    public partial class Exhibition
    {
        public string HallTitle => HallFkNavigation?.Title ?? "";
        public string CuratorFullName => ResponsibleCuratorFkNavigation?.FullName ?? "";
        public string ExhibitionStatusTitle => ExhibitionStatusFkNavigation?.Title ?? "";
        public string ExhibitsString => ExhibitOnExhibitions == null ? "" :
            string.Join(", ", ExhibitOnExhibitions
                .Where(eoe => eoe.ExhibitFkNavigation != null)
                .Select(eoe => eoe.ExhibitFkNavigation.Title)
                .Distinct());
    }
}
