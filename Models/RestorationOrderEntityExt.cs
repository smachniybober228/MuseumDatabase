using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Museum.Models
{
    public partial class RestorationOrderEntity
    {
        public string ExhibitTitle => ExhibitFkNavigation?.Title ?? "";
        public string RestorerName => RestorerFkNavigation?.FullName ?? "";
        public string InitiatorName => InitiatorFkNavigation?.FullName ?? "";
        public string RequiredWorksString => RequiredWorkTypes == null ? "" :
            string.Join(", ", RequiredWorkTypes.Select(rwt => rwt.WorkTypeFkNavigation?.Title ?? ""));

        [NotMapped]
        public string StatusText { get; set; } // вычисляется в ViewModel
    }
}
